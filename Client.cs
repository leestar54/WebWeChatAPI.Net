using Leestar54.WeChat.WebAPI.Modal;
using Leestar54.WeChat.WebAPI.Modal.Request;
using Leestar54.WeChat.WebAPI.Modal.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Leestar54.WeChat.WebAPI
{
    /// <summary>
    /// 过千人账号有时候获取不到联系人列表，服务器返回503，官方测试结果也是反馈503导致获取不到
    /// </summary>
    public class GetContactException : Exception
    {
        public GetContactException(string msg) : base(msg) { }
    }

    /// <summary>
    /// 异步操作失败异常
    /// </summary>
    public class OperateFailException : Exception
    {
        public OperateFailException(string msg) : base(msg) { }
    }

    public class Client
    {
        private AsyncOperation asyncOperation;
        private TaskFactory factory;
        private CancellationTokenSource source;
        private HttpClient httpClient = new HttpClient();
        private bool finishGetContactList = false;

        private bool syncPolling = true;

        #region 重要内部参数
        private string passTicket;
        /// <summary>
        /// 扫码之后返回要跳转跳转页面，通过这个地址获取cookie等总要信息
        /// </summary>
        private string cookieRedirectUri;
        private SyncKey syncKey;
        private BaseRequest baseRequest;
        private User user;
        /// <summary>
        /// 检测是否扫码用
        /// </summary>
        private string uuid = string.Empty;

        private string host = "https://wx2.qq.com";
        private string pushHost = "https://webpush.wx2.qq.com";
        private string uploadHost = "https://file.wx2.qq.com";

        public string Host
        {
            get
            {
                return this.host;
            }
        }
        /// <summary>
        /// 当前登录用户
        /// </summary>
        public User CurrentUser
        {
            get
            {
                return user;
            }
        }

        /// <summary>
        /// 是否读取完初始化联系人列表
        /// </summary>
        public bool IsFinishGetContactList
        {
            get
            {
                return finishGetContactList;
            }

            set
            {
                finishGetContactList = value;
            }
        }
        #endregion

        private List<MPSubscribeMsg> mpSubscribeMsgList = new List<MPSubscribeMsg>();
        private Dictionary<string, string> uploadMedia = new Dictionary<string, string>();

        #region 异步回调事件
        /// <summary>
        /// 异步调用的异常都会反馈在这里。
        /// </summary>
        public event EventHandler<TEventArgs<Exception>> ExceptionCatched;
        /// <summary>
        /// 获取登陆二维码
        /// </summary>
        public event EventHandler<TEventArgs<Image>> GetLoginQrCodeComplete;
        /// <summary>
        /// 用户扫码
        /// </summary>
        public event EventHandler<TEventArgs<Image>> CheckScanComplete;
        /// <summary>
        /// 登陆成功，返回当前登录用户信息
        /// </summary>
        public event EventHandler<TEventArgs<User>> LoginComplete;
        /// <summary>
        /// 批次读取联系人信息
        /// </summary>
        public event EventHandler<TEventArgs<List<Contact>>> BatchGetContactComplete;
        /// <summary>
        /// 获取联系人列表
        /// </summary>
        public event EventHandler<TEventArgs<List<Contact>>> GetContactComplete;
        /// <summary>
        /// 登出
        /// </summary>
        public event EventHandler<TEventArgs<User>> LogoutComplete;
        /// <summary>
        /// 接受消息
        /// </summary>
        public event EventHandler<TEventArgs<List<AddMsg>>> ReceiveMsg;
        /// <summary>
        /// 公众号文章读取完成
        /// </summary>
        public event EventHandler<TEventArgs<List<MPSubscribeMsg>>> MPSubscribeMsgListComplete;
        /// <summary>
        /// 删除联系人完成
        /// </summary>
        public event EventHandler<TEventArgs<List<DelContactItem>>> DelContactListComplete;
        /// <summary>
        /// 修改联系人完成
        /// </summary>
        public event EventHandler<TEventArgs<List<Contact>>> ModContactListComplete;
        //public event EventHandler<TEventArgs<List<>>> ModChatRoomMemberListComplete;
        #endregion

        public Client()
        {
            baseRequest = new BaseRequest();
            asyncOperation = AsyncOperationManager.CreateOperation(null);
            source = new CancellationTokenSource();
            factory = new TaskFactory(source.Token);
        }
        /// <summary>
        /// 启动客户端
        /// </summary>
        /// <param name="lastCookie">上次保留的登录信息。</param>
        public void Start(string lastCookie = null)
        {
            if (syncPolling != false)
            {
                factory.StartNew(() => { if (!AutoLogin(lastCookie)) GetLoginQrCode(); })
                    .ContinueWith((antecedent) => CheckSacnLogin(), source.Token, TaskContinuationOptions.NotOnFaulted, TaskScheduler.Default)
                    .ContinueWith((antecedent) => Init(), source.Token, TaskContinuationOptions.NotOnFaulted, TaskScheduler.Default)
                    .ContinueWith((antecedent) => StatusNotify(), source.Token, TaskContinuationOptions.NotOnFaulted, TaskScheduler.Default)
                    .ContinueWith((antecedent) => GetContact(), source.Token, TaskContinuationOptions.NotOnFaulted, TaskScheduler.Default)
                    .ContinueWith((antecedent) => Sync(), source.Token, TaskContinuationOptions.NotOnFaulted, TaskScheduler.Default);
            }
            else
            {
                throw new ObjectDisposedException("Client", "客户端已经登出释放，请重新实例化。");
            }
        }
        /// <summary>
        /// 尝试自动登录
        /// </summary>
        /// <param name="lastCookie">上次保留的登录信息。</param>
        /// <returns></returns>
        private bool AutoLogin(string lastCookie)
        {
            try
            {
                if (string.IsNullOrEmpty(lastCookie)) return false;

                string url = host + "/cgi-bin/mmwebwx-bin/webwxpushloginurl?uin=";
                httpClient.CookieContainer.SetCookies(new Uri(url), lastCookie.Replace(";", ","));
                string uin = httpClient.CookieContainer.GetCookies(new Uri(url))["wxuin"].Value;
                if (string.IsNullOrEmpty(uin)) return false;

                string result = httpClient.GetString(url + uin);
                JObject o = (JObject)JsonConvert.DeserializeObject(result);
                if (o["ret"].ToString() != "0") return false;
                uuid = o["uuid"].ToString();

                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 获取新的的登录信息。
        /// </summary>
        /// <returns></returns>
        public string GetLastCookie()
        {
            var cookies = httpClient.CookieContainer.GetCookies(new Uri(host));
            return $"last_wxuin={cookies["wxuin"].Value};wxuin={cookies["wxuin"].Value};" +
                $"webwxuvid={cookies["webwxuvid"].Value}; webwx_auth_ticket={cookies["webwx_auth_ticket"].Value}";
        }

        /// <summary>
        /// 如果还未sync，调用此方法结束客户端，否则调用logout
        /// </summary>
        public void Close()
        {
            syncPolling = false;
            source.Cancel();
        }

        /// <summary>
        /// 同步登出客户端
        /// </summary>
        /// <returns>可以不用理会结果</returns>
        public string Logout()
        {
            string logoutUrl = string.Format(host + "/cgi-bin/mmwebwx-bin/webwxlogout?redirect=1&type=0&skey={0}", baseRequest.Skey);
            string body = string.Format("sid={0}&uin={1}", baseRequest.Sid, baseRequest.Uin);
            string result = string.Empty;
            result = httpClient.PostFormString(logoutUrl, body);
            OtherUtils.Debug(result, "logout");
            Close();
            return result;
        }

        /// <summary>
        /// 异步登出客户端
        /// </summary>
        public void LogoutAsync()
        {
            Task.Factory.StartNew(() =>
            {
                if (baseRequest != null)
                {
                    try
                    {
                        string result = Logout();
                        asyncOperation.Post(
                        new SendOrPostCallback((obj) =>
                        {
                            LogoutComplete?.Invoke(this, new TEventArgs<User>((User)obj));
                        }), user);
                    }
                    catch (Exception e)
                    {
                        asyncOperation.Post(
                        new SendOrPostCallback((obj) =>
                        {
                            ExceptionCatched?.Invoke(this, new TEventArgs<Exception>((Exception)obj));
                        }), e);
                        Close();
                    }
                }
            });
        }

        /// <summary>
        /// 获取登陆二维码
        /// </summary>
        private void GetLoginQrCode()
        {
            try
            {
                string jsloginUrl = "https://login.wx.qq.com/jslogin?appid=wx782c26e4c19acffb&redirect_uri=https%3A%2F%2Fwx.qq.com%2Fcgi-bin%2Fmmwebwx-bin%2Fwebwxnewloginpage&fun=new&lang=en_US&_=" + OtherUtils.GetJavaTimeStamp();
                string result = httpClient.GetString(jsloginUrl);
                OtherUtils.Debug("GetLoginQrCode " + result);
                string qruuidStr = "window.QRLogin.uuid = \"";
                int index = result.IndexOf("window.QRLogin.uuid = \"");
                if (index == -1)
                {
                    throw new Exception("获取登陆二维码失败，请稍后再试。");
                }
                else
                {
                    uuid = result.Substring(index + qruuidStr.Length, result.Length - index - qruuidStr.Length - "\";".Length);
                }
                string qrcodeUrl = string.Format("https://login.weixin.qq.com/qrcode/{0}", uuid);
                Image img = httpClient.GetImage(qrcodeUrl);
                asyncOperation.Post(
                new SendOrPostCallback((obj) =>
                {
                    GetLoginQrCodeComplete?.Invoke(this, new TEventArgs<Image>((Image)obj));
                }), img);
            }
            catch (Exception e)
            {
                asyncOperation.Post(
                new SendOrPostCallback((obj) =>
                {
                    ExceptionCatched?.Invoke(this, new TEventArgs<Exception>((Exception)obj));
                }), e);
                Close();
            }
        }

        private enum ScanState { UnKnown, Timeout, Scan, Login, Expires };

        /// <summary>
        /// 检测手机是否扫码
        /// </summary>
        private void CheckSacnLogin()
        {
            try
            {
                Image userAvatar = null;
                ScanState scanState = ScanState.UnKnown;
                while (syncPolling && (scanState != ScanState.Login))
                {

                    string timespan = OtherUtils.GetTimeStamp();
                    string loginUrl = string.Format("https://login.wx.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid={0}&tip=0&r={1}&_={2}", uuid, OtherUtils.Get_r(), OtherUtils.GetTimeStamp());
                    //采用长轮询的方式，25秒返内回一次检测数据。
                    string checkResult = httpClient.GetString(loginUrl);
                    OtherUtils.Debug("CheckSacnLogin " + checkResult);
                    if (checkResult.IndexOf("window.code=408;") != -1)
                    {
                        scanState = ScanState.Timeout;
                    }
                    else if (checkResult.IndexOf("window.code=201;") != -1)
                    {
                        scanState = ScanState.Scan;
                        //有些号没有头像就跳过这个步骤
                        if (checkResult.IndexOf("window.userAvatar") != -1)
                        {
                            //扫码返回的头像是base64格式，需要转化
                            string subStr = "window.code=201;window.userAvatar = 'data:img/jpg;base64,";
                            string base64UserAvatar = checkResult.Substring(subStr.Length, checkResult.Length - subStr.Length - 2);
                            byte[] arr = Convert.FromBase64String(base64UserAvatar);
                            using (MemoryStream ms = new MemoryStream(arr))
                            {
                                userAvatar = Image.FromStream(ms);
                            }
                            asyncOperation.Post(
                                new SendOrPostCallback((obj) =>
                                {
                                    CheckScanComplete?.Invoke(this, new TEventArgs<Image>((Image)obj));
                                }), userAvatar);
                        }
                    }
                    else if (checkResult.IndexOf("window.code=200;") != -1)
                    {
                        scanState = ScanState.Login;
                        string subStr = "window.code=200;\nwindow.redirect_uri=\"";
                        cookieRedirectUri = checkResult.Substring(subStr.Length, checkResult.Length - subStr.Length - 2);
                        //跳转登录页获取cookie，并且获取关键参数，根据跳转地址，获相应提交地址
                        string cookieRedirectResult = httpClient.GetString(cookieRedirectUri);
                        if (cookieRedirectUri.StartsWith("https://wx2.qq.com"))
                        {
                            host = "https://wx2.qq.com";
                            pushHost = "https://webpush.wx2.qq.com";
                            uploadHost = "https://file.wx2.qq.com";
                        }
                        else if (cookieRedirectUri.StartsWith("https://wx8.qq.com"))
                        {
                            host = "https://wx8.qq.com";
                            pushHost = "https://webpush.wx8.qq.com";
                            uploadHost = "https://file.wx8.qq.com";
                        }
                        else if (cookieRedirectUri.StartsWith("https://web2.wechat.com"))
                        {
                            host = "https://web2.wechat.com";
                            pushHost = "https://webpush.web2.wechat.com";
                            uploadHost = "https://file.web2.wechat.com";
                        }
                        else if (cookieRedirectUri.StartsWith("https://web.wechat.com"))
                        {
                            host = "https://web.wechat.com";
                            pushHost = "https://webpush.web.wechat.com";
                            uploadHost = "https://file.web.wechat.com";
                        }
                        else
                        {
                            host = "https://wx.qq.com";
                            pushHost = "https://webpush.wx.qq.com";
                            uploadHost = "https://file.wx.qq.com";
                        }

                        httpClient.Referer = host;
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(cookieRedirectResult);
                        //如果返回异常，则可能被暂封，无法登陆网页版
                        if (xmlDoc["error"]["ret"].InnerText != "0")
                        {
                            throw new Exception(xmlDoc["error"]["message"].InnerText);
                        }
                        else
                        {
                            baseRequest.Sid = xmlDoc["error"]["wxsid"].InnerText;
                            baseRequest.Uin = Convert.ToInt64(xmlDoc["error"]["wxuin"].InnerText);
                            baseRequest.Skey = xmlDoc["error"]["skey"].InnerText;
                            passTicket = xmlDoc["error"]["pass_ticket"].InnerText;
                        }
                    }
                    else if (checkResult.IndexOf("window.code=400;") != -1)
                    {
                        scanState = ScanState.Expires;
                        GetLoginQrCode();
                    }
                    else
                    {
                        scanState = ScanState.UnKnown;
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                asyncOperation.Post(
                new SendOrPostCallback((obj) =>
                {
                    ExceptionCatched?.Invoke(this, new TEventArgs<Exception>((Exception)obj));
                }), e);
                Close();
            }
        }

        /// <summary>
        /// 开始初始化所有关键内容
        /// </summary>
        private void Init()
        {
            try
            {
                string webwxinitUrl = string.Format(host + "/cgi-bin/mmwebwx-bin/webwxinit?r={0}", OtherUtils.Get_r());
                JObject postjson = JObject.FromObject(new
                {
                    BaseRequest = baseRequest
                });
                InitResponse initMsg = httpClient.PostJson<InitResponse>(webwxinitUrl, postjson);
                if (initMsg.BaseResponse.Ret != 0)
                {
                    throw new Exception("程序初始化失败");
                }
                //初始化2次，官网也是初始化2次，这样貌似比较稳定
                httpClient.PostJson<InitResponse>(webwxinitUrl, postjson);
                user = initMsg.User;
                mpSubscribeMsgList = initMsg.MPSubscribeMsgList;
                syncKey = initMsg.SyncKey;

                //初始化的时候会返回一个最近联系人列表，但是主要还是以第一次sync获得的最近联系人为准。
                asyncOperation.Post(
                    new SendOrPostCallback((list) =>
                    {
                        BatchGetContactComplete?.Invoke(this, new TEventArgs<List<Contact>>((List<Contact>)list));
                    }), initMsg.ContactList);

                asyncOperation.Post(
                new SendOrPostCallback((obj) =>
                {
                    LoginComplete?.Invoke(this, new TEventArgs<User>((User)obj));
                }), user);
            }
            catch (Exception e)
            {
                asyncOperation.Post(
                new SendOrPostCallback((obj) =>
                {
                    ExceptionCatched?.Invoke(this, new TEventArgs<Exception>((Exception)obj));
                }), e);
                Close();
            }
        }

        /// <summary>
        /// 主要用于提醒手机端，同步状态
        /// </summary>
        private void StatusNotify()
        {
            try
            {
                //反馈服务器
                string webwxstatusnotifyUrl = host + "/cgi-bin/mmwebwx-bin/webwxstatusnotify";
                StatusNotifyRequest statusNotifyRequest = new StatusNotifyRequest();
                statusNotifyRequest.BaseRequest = baseRequest;
                statusNotifyRequest.Code = 3;
                statusNotifyRequest.FromUserName = user.UserName;
                statusNotifyRequest.ToUserName = user.UserName;
                statusNotifyRequest.ClientMsgId = OtherUtils.GetJavaTimeStamp();
                //反馈结果可以不理
                httpClient.PostJson<StatusNotifyResponse>(webwxstatusnotifyUrl, statusNotifyRequest);
            }
            catch (Exception e)
            {
                asyncOperation.Post(
                new SendOrPostCallback((obj) =>
                {
                    ExceptionCatched?.Invoke(this, new TEventArgs<Exception>((Exception)obj));
                }), e);
                Close();
            }
        }

        /// <summary>
        /// 拉取联系人信息。
        /// </summary>
        /// <param name="statusNotifyUserName">需要获取的UserName列表，包括群，个人用户，用英文,分割</param>
        /// <param name="EncryChatRoomId">默认为空，如果是获取群内成员详细信息，则填写encryChatRoomId，也就是群的UserName</param>
        public void GetBatchGetContactAsync(string statusNotifyUserName, string encryChatRoomId = "")
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    //获取历史会话列表
                    string webwxbatchgetcontactUrl = string.Format(host + "/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r={0}", OtherUtils.GetJavaTimeStamp());
                    string[] chatNameArr = statusNotifyUserName.Split(',');
                    bool finishGetChatList = false;
                    BatchGetContactRequest batchGetContactRequest = new BatchGetContactRequest();
                    batchGetContactRequest.BaseRequest = baseRequest;
                    int count = chatNameArr.Length;
                    int index = 0;
                    //一批次最多获取50条，多出来分批获取
                    while (!finishGetChatList)
                    {
                        batchGetContactRequest.List = new List<ChatRoom>();
                        if (((index + 1) * 50) < count)
                        {
                            for (int i = index * 50; i < (index + 1) * 50; i++)
                            {
                                batchGetContactRequest.List.Add(new ChatRoom { UserName = chatNameArr[i], EncryChatRoomId = encryChatRoomId });
                            }
                        }
                        else
                        {
                            for (int i = index * 50; i < count; i++)
                            {
                                batchGetContactRequest.List.Add(new ChatRoom { UserName = chatNameArr[i], EncryChatRoomId = encryChatRoomId });
                            }
                            finishGetChatList = true;
                        }
                        BatchGetContactResponse batchGetContactMsg = httpClient.PostJson<BatchGetContactResponse>(webwxbatchgetcontactUrl, batchGetContactRequest);
                        asyncOperation.Post(
                        new SendOrPostCallback((list) =>
                        {
                            BatchGetContactComplete?.Invoke(this, new TEventArgs<List<Contact>>((List<Contact>)list));
                        }), batchGetContactMsg.ContactList);

                        index++;
                    }
                }
                catch (Exception e)
                {
                    asyncOperation.Post(
                    new SendOrPostCallback((obj) =>
                    {
                        ExceptionCatched?.Invoke(this, new TEventArgs<Exception>((Exception)obj));
                    }), e);
                }
            });
        }
        /// <summary>
        /// 读取用户的联系人列表，其中只包含公众号，个人号，如果返回值seq不为0，那么用户列表还没获取完（因为可能会有几千人的号，不可能一次获取完），则附带上seq的值继续获取。
        /// </summary>
        private void GetContact()
        {
            try
            {
                //获取联系人列表
                finishGetContactList = false;
                string getContactUrl = string.Format(host + "/cgi-bin/mmwebwx-bin/webwxgetcontact?r={0}&seq={1}&skey={2}", OtherUtils.GetJavaTimeStamp(), 0, baseRequest.Skey);
                while (!finishGetContactList)
                {
                    string contactResult = httpClient.GetStringOnce(getContactUrl);
                    GetContactResponse getContactResponse = JsonConvert.DeserializeObject<GetContactResponse>(contactResult);
                    asyncOperation.Post(
                    new SendOrPostCallback((list) =>
                    {
                        GetContactComplete?.Invoke(this, new TEventArgs<List<Contact>>((List<Contact>)list));
                    }), getContactResponse.MemberList);

                    if (getContactResponse.Seq == 0)
                    {
                        finishGetContactList = true;
                    }
                    else
                    {
                        getContactUrl = string.Format(host + "/cgi-bin/mmwebwx-bin/webwxgetcontact?r={0}&seq={1}&skey={2}", OtherUtils.GetJavaTimeStamp(), getContactResponse.Seq, baseRequest.Skey);
                    }
                }

                //获取完联系人中的公众号，才能获得名称，这个时候再发送图文消息事件。
                asyncOperation.Post(
                new SendOrPostCallback((obj) =>
                {
                    MPSubscribeMsgListComplete?.Invoke(this, new TEventArgs<List<MPSubscribeMsg>>((List<MPSubscribeMsg>)obj));
                }), mpSubscribeMsgList);
            }
            catch (Exception e)
            {
                if (e is WebException)
                {

                    WebException we = e as WebException;
                    if (we.Status == WebExceptionStatus.ProtocolError && ((HttpWebResponse)we.Response).StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        //过千人账号有时候获取不到联系人列表，服务器返回503，官方测试结果也是反馈503导致获取不到，为了不影响正常使用，跳过获取联系人步骤
                        asyncOperation.Post(
                        new SendOrPostCallback((obj) =>
                        {
                            ExceptionCatched?.Invoke(this, new TEventArgs<Exception>((Exception)obj));
                        }), new GetContactException("无法获取好友列表"));
                    }
                }
                else
                {
                    asyncOperation.Post(
                    new SendOrPostCallback((obj) =>
                    {
                        ExceptionCatched?.Invoke(this, new TEventArgs<Exception>((Exception)obj));
                    }), e);
                }
            }
        }

        /// <summary>
        /// 开始轮询检测是否有新消息
        /// </summary>
        private void Sync()
        {
            try
            {
                while (syncPolling)
                {
                    string syncCheckUrl = string.Format(pushHost + "/cgi-bin/mmwebwx-bin/synccheck?r={0}&skey={1}&sid={2}&uin={3}&deviceid={4}&synckey={5}&_={6}", OtherUtils.GetJavaTimeStamp(), baseRequest.Skey, baseRequest.Sid, baseRequest.Uin, baseRequest.DeviceID, syncKey.ToString(), syncKey.Step);
                    string syncCheckResult = httpClient.GetString(syncCheckUrl);
                    if (!syncPolling)
                    {
                        return;
                    }
                    MatchCollection matchCollection = Regex.Matches(syncCheckResult, @"\d+");
                    //0 正常
                    string retcode = matchCollection[0].Value;
                    //0 正常
                    //2 新的消息
                    //4 通过时发现，删除好友
                    //6 删除时发现和对方通过好友验证
                    //7 进入 / 离开聊天界面 （可能没有了）
                    string selector = matchCollection[1].Value;
                    OtherUtils.Debug("retcode:" + retcode + " selector:" + selector);
                    switch (retcode)
                    {
                        case "0":
                            if (selector != "0")
                            {
                                //有新消息，拉取信息。
                                SyncRequest syncRequest = new SyncRequest();
                                syncRequest.BaseRequest = baseRequest;
                                syncRequest.SyncKey = syncKey;
                                syncRequest.rr = OtherUtils.Get_r();
                                string syncUrl = string.Format(host + "/cgi-bin/mmwebwx-bin/webwxsync?sid={0}&skey={1}", baseRequest.Sid, baseRequest.Skey);
                                SyncResponse syncResponse = httpClient.PostJson<SyncResponse>(syncUrl, syncRequest);
                                if (!syncPolling)
                                {
                                    return;
                                }
                                else
                                {
                                    if (syncResponse.SyncKey.Count > 0)
                                    {
                                        syncKey = syncResponse.SyncKey;
                                    }
                                    //只要不是0，就是有消息，有消息我们处理就行了，不管selector是几
                                    if (syncResponse.AddMsgCount == 0 && syncResponse.DelContactCount == 0 && syncResponse.ModContactCount == 0 && syncResponse.ModChatRoomMemberCount == 0)
                                    {
                                        //会有这么一种情况，selector=2，但是没有任何消息体，这样会导致持续快速的空交互
                                        //除非下次有新消息，或者主动点击手机触发消息
                                        //为了防止这种情况，做个5秒停顿。
                                        Thread.Sleep(5000);
                                    }
                                    else
                                    {
                                        if (syncResponse.AddMsgList.Count > 0)
                                        {
                                            asyncOperation.Post(
                                           new SendOrPostCallback((obj) =>
                                           {
                                               ReceiveMsg?.Invoke(this, new TEventArgs<List<AddMsg>>((List<AddMsg>)obj));
                                           }), syncResponse.AddMsgList);
                                        }

                                        if (syncResponse.ModContactCount > 0)
                                        {
                                            asyncOperation.Post(
                                           new SendOrPostCallback((obj) =>
                                           {
                                               ModContactListComplete?.Invoke(this, new TEventArgs<List<Contact>>((List<Contact>)obj));
                                           }), syncResponse.ModContactList);
                                        }
                                        if (syncResponse.DelContactCount > 0)
                                        {
                                            asyncOperation.Post(
                                           new SendOrPostCallback((obj) =>
                                           {
                                               DelContactListComplete?.Invoke(this, new TEventArgs<List<DelContactItem>>((List<DelContactItem>)obj));
                                           }), syncResponse.DelContactList);
                                        }
                                        if (syncResponse.ModChatRoomMemberCount > 0)
                                        {
                                            //待分析，这个消息基本没有
                                        }
                                    }
                                }
                            }
                            break;
                        case "1100":
                            //登出了微信，很可能是wx.qq.com和wx2.qq.com调用接口不一致导致的，注意登陆时候的跳转地址
                            Close();
                            asyncOperation.Post(
                            new SendOrPostCallback((obj) =>
                            {
                                LogoutComplete?.Invoke(this, new TEventArgs<User>((User)obj));
                            }), user);
                            break;
                        case "1101":
                            Close();
                            asyncOperation.Post(
                            new SendOrPostCallback((obj) =>
                            {
                                LogoutComplete?.Invoke(this, new TEventArgs<User>((User)obj));
                            }), user);
                            throw new Exception("1101可能其他地方登录/登出了 WEB 版微信，请检查手机端已登出WEB微信，然后稍后再试");
                            break;
                        case "1102":
                            Close();
                            asyncOperation.Post(
                            new SendOrPostCallback((obj) =>
                            {
                                LogoutComplete?.Invoke(this, new TEventArgs<User>((User)obj));
                            }), user);
                            throw new Exception("1102被强制登出（很可能cookie冲突），请检查手机端已登出WEB微信，然后稍后再试");
                            break;
                        default:
                            //有其他任何异常，取消轮询
                            throw new Exception("轮询结果异常，停止轮询:" + syncCheckResult);
                            break;
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                asyncOperation.Post(
                new SendOrPostCallback((obj) =>
                {
                    ExceptionCatched?.Invoke(this, new TEventArgs<Exception>((Exception)obj));
                }), e);
            }
        }

        /// <summary>
        /// 同步发送文字消息
        /// </summary>
        /// <param name="msg">文字</param>
        /// <param name="toUserName">发送人UserName</param>
        /// <returns></returns>
        public SendMsgResponse SendMsg(string msg, string toUserName)
        {
            string time = OtherUtils.GetJavaTimeStamp().ToString();
            string sendMsgUrl = string.Format(host + "/cgi-bin/mmwebwx-bin/webwxsendmsg?lang=zh_CN&pass_ticket={0}", passTicket);
            SendMsgRequest sendMsgRequest = new SendMsgRequest()
            {
                BaseRequest = baseRequest,
                Msg = new Msg()
                {
                    FromUserName = user.UserName,
                    ToUserName = toUserName,
                    ClientMsgId = time,
                    LocalID = time,
                    Type = MSGTYPE.MSGTYPE_TEXT,
                    Content = msg
                },
                Scene = 0
            };
            SendMsgResponse sendMsgResponse = httpClient.PostJson<SendMsgResponse>(sendMsgUrl, sendMsgRequest);
            return sendMsgResponse;
        }

        /// <summary>
        /// 同步发送文件，自动分块上传，文件较大可能会卡住进程，建议异步发送
        /// </summary>
        /// <param name="fileInfo">文件信息</param>
        /// <param name="toUserName">发送人UserName</param>
        /// <returns></returns>
        public SendMsgResponse SendMsg(FileInfo fileInfo, string toUserName)
        {
            string mediaId = string.Empty;
            if (uploadMedia.Keys.Contains(fileInfo.Name))
            {
                mediaId = uploadMedia[fileInfo.Name];
            }
            else
            {
                UploadMediaResponse uploadMediaResponse = UploadFile(fileInfo, toUserName);
                mediaId = uploadMediaResponse.MediaId;
            }
            string mime = MimeMapping.GetMimeMapping(fileInfo.Name);
            string time = OtherUtils.GetJavaTimeStamp().ToString();
            SendMsgResponse response = null;
            if (mime.StartsWith("image"))
            {
                string sendMsgUrl = string.Format(host + "/cgi-bin/mmwebwx-bin/webwxsendmsgimg?fun=async&f=json&lang=zh_CN&pass_ticket={0}", passTicket);
                SendMediaMsgRequest sendImgMsgRequest = new SendMediaMsgRequest()
                {
                    BaseRequest = baseRequest,
                    Msg = new MediaMsg()
                    {
                        ClientMsgId = time,
                        FromUserName = user.UserName,
                        LocalID = time,
                        MediaId = mediaId,
                        ToUserName = toUserName,
                        Type = MSGTYPE.MSGTYPE_IMAGE
                    },
                    Scene = 0
                };
                response = httpClient.PostJson<SendMsgResponse>(sendMsgUrl, sendImgMsgRequest);
            }
            else if (mime.StartsWith("video"))
            {
                string sendMsgUrl = string.Format(host + "/cgi-bin/mmwebwx-bin/webwxsendvideomsg?fun=async&f=json&lang=zh_CN&pass_ticket={0}", passTicket);
                SendMediaMsgRequest sendImgMsgRequest = new SendMediaMsgRequest()
                {
                    BaseRequest = baseRequest,
                    Msg = new MediaMsg()
                    {
                        ClientMsgId = time,
                        FromUserName = user.UserName,
                        LocalID = time,
                        MediaId = mediaId,
                        ToUserName = toUserName,
                        Type = MSGTYPE.MSGTYPE_IMAGE
                    },
                    Scene = 0
                };
                response = httpClient.PostJson<SendMsgResponse>(sendMsgUrl, sendImgMsgRequest);
            }
            else
            {
                string sendMsgUrl = string.Format(host + "/cgi-bin/mmwebwx-bin/webwxsendappmsg?fun=async&f=json&lang=zh_CN&pass_ticket={0}", passTicket);
                SendMsgRequest sendAppMsgRequest = new SendMsgRequest()
                {
                    BaseRequest = baseRequest,
                    Msg = new Msg()
                    {
                        ClientMsgId = time,
                        Content = string.Format("<appmsg appid='wxeb7ec651dd0aefa9' sdkver=''><title>{0}</title><des></des><action></action><type>6</type><content></content><url></url><lowurl></lowurl><appattach><totallen>{1}</totallen><attachid>{2}</attachid><fileext>{3}</fileext></appattach><extinfo></extinfo></appmsg>", fileInfo.Name, fileInfo.Length, mediaId, fileInfo.Extension.Substring(1)),
                        FromUserName = user.UserName,
                        ToUserName = toUserName,
                        LocalID = time,
                        Type = MSGTYPE.MSGTYPE_DOC
                    },
                    Scene = 0
                };
                response = httpClient.PostJson<SendMsgResponse>(sendMsgUrl, sendAppMsgRequest);
            }
            return response;
        }

        /// <summary>
        /// 异步发送文件
        /// </summary>
        /// <param name="fileInfo">文件信息</param>
        /// <param name="toUserName">发送人UserName</param>
        public void SendMsgAsync(FileInfo fileInfo, string toUserName)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    SendMsgResponse response = SendMsg(fileInfo, toUserName);
                    if (response.BaseResponse.Ret != 0)
                    {
                        asyncOperation.Post(
                        new SendOrPostCallback((obj) =>
                        {
                            ExceptionCatched?.Invoke(this, new TEventArgs<Exception>((Exception)obj));
                        }), new OperateFailException(response.BaseResponse.ErrMsg));
                    }
                }
                catch (Exception e)
                {
                    asyncOperation.Post(
                    new SendOrPostCallback((obj) =>
                    {
                        ExceptionCatched?.Invoke(this, new TEventArgs<Exception>((Exception)obj));
                    }), e);
                }
            });
        }

        /// <summary>
        /// 异步发送文字消息
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="toUserName">发送人UserName</param>
        public void SendMsgAsync(string msg, string toUserName)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    SendMsgResponse response = SendMsg(msg, toUserName);
                    if (response.BaseResponse.Ret != 0)
                    {
                        asyncOperation.Post(
                        new SendOrPostCallback((obj) =>
                        {
                            ExceptionCatched?.Invoke(this, new TEventArgs<Exception>((Exception)obj));
                        }), new OperateFailException(response.BaseResponse.ErrMsg));
                    }
                }
                catch (Exception e)
                {
                    asyncOperation.Post(
                    new SendOrPostCallback((obj) =>
                    {
                        ExceptionCatched?.Invoke(this, new TEventArgs<Exception>((Exception)obj));
                    }), e);
                }
            });
        }
        /// <summary>
        /// 获取头像，因为请求的时候需要带Cookie等相关参数，所以直接用新的http请求不行，务必使用客户端API来获取
        /// </summary>
        /// <param name="url">头像地址，例如/cgi-bin/mmwebwx-bin/webwxgeticon?seq=0&username=filehelper&skey=@crypt_372b266_540d016177e861740ee84fec697a3b01 </param>
        /// <param name="action">委托Action</param>
        /// <returns></returns>
        public void GetIconAsync(string url, Action<Image> action)
        {
            string fullUrl = host + url;
            new Task(() =>
            {
                try
                {
                    Image img = httpClient.GetImage(fullUrl);
                    asyncOperation.Post(
                    new SendOrPostCallback((obj) =>
                    {
                        action((Image)obj);
                    }), img);
                }
                catch (Exception e)
                {
                    asyncOperation.Post(
                    new SendOrPostCallback((obj) =>
                    {
                        ExceptionCatched?.Invoke(this, new TEventArgs<Exception>((Exception)obj));
                    }), e);
                }
            }).Start();
        }

        /// <summary>
        /// 同步上传文件
        /// </summary>
        /// <param name="fileInfo">文件信息</param>
        /// <returns></returns>
        public UploadMediaResponse UploadFile(FileInfo fileInfo)
        {
            return UploadFile(fileInfo, user.UserName);
        }

        /// <summary>
        /// 同步上传文件
        /// </summary>
        /// <param name="fileInfo">文件信息</param>
        /// <param name="toUserName">发送人UserName，其实没什么用，但是官方有这个参数</param>
        /// <returns></returns>
        public UploadMediaResponse UploadFile(FileInfo fileInfo, string toUserName)
        {
            string postUrl = uploadHost + "/cgi-bin/mmwebwx-bin/webwxuploadmedia?f=json";
            int bufferLength = 512 * 1024;
            string datetime = DateTime.Now.ToString("ddd MMM dd yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US")) + " GMT+0800 (中国标准时间)";
            UploadMediaRequest uploadMediaRequest = new UploadMediaRequest()
            {
                UploadType = 2,
                BaseRequest = baseRequest,
                ClientMediaId = OtherUtils.GetJavaTimeStamp(),
                TotalLen = fileInfo.Length,
                StartPos = 0,
                DataLen = fileInfo.Length,
                MediaType = 4,
                FromUserName = user.UserName,
                ToUserName = toUserName,
                FileMd5 = OtherUtils.GetFileMD5Hash(fileInfo)
            };
            UploadMediaResponse response = null;
            //文件大小超过512Kb，分块上传。
            if (fileInfo.Length > bufferLength)
            {
                int chunks = (int)Math.Ceiling((double)fileInfo.Length / bufferLength);
                byte[] buffer = new byte[bufferLength];
                Stream readStream = fileInfo.OpenRead();
                int chunk = 0;
                int readLength = 0;
                while ((readLength = readStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    List<FormDataItem> dataList = new List<FormDataItem>()
                    {
                        new FormDataItem("id","WU_FILE_"+uploadMedia.Count),
                        new FormDataItem("name",fileInfo.Name),
                        new FormDataItem("type",MimeMapping.GetMimeMapping(fileInfo.Name)),
                        new FormDataItem("lastModifiedDate",datetime),
                        new FormDataItem("size",fileInfo.Length.ToString()),
                        new FormDataItem("chunks",chunks.ToString()),
                        new FormDataItem("chunk",chunk.ToString()),
                        new FormDataItem("mediatype",GetMediaType(fileInfo.Extension)),
                        new FormDataItem("uploadmediarequest",JsonConvert.SerializeObject(uploadMediaRequest, Newtonsoft.Json.Formatting.None)),
                        new FormDataItem("webwx_data_ticket",httpClient.CookieContainer.GetCookies(new Uri(cookieRedirectUri))["webwx_data_ticket"].Value),
                        new FormDataItem("pass_ticket",passTicket),
                        new FormDataItem("filename",fileInfo.Name,buffer,readLength)
                    };
                    string result = httpClient.PostMutipart(postUrl, dataList);
                    response = JsonConvert.DeserializeObject<UploadMediaResponse>(result);
                    chunk++;
                }
            }
            else
            {
                byte[] buffer = new byte[fileInfo.Length];
                Stream readStream = fileInfo.OpenRead();
                int readLength = readStream.Read(buffer, 0, buffer.Length);
                List<FormDataItem> dataList = new List<FormDataItem>()
                    {
                        new FormDataItem("id","WU_FILE_"+uploadMedia.Count),
                        new FormDataItem("name",fileInfo.Name),
                        new FormDataItem("type",MimeMapping.GetMimeMapping(fileInfo.Name)),
                        new FormDataItem("lastModifiedDate",datetime),
                        new FormDataItem("size",fileInfo.Length.ToString()),
                        new FormDataItem("mediatype",GetMediaType(fileInfo.Extension)),
                        new FormDataItem("uploadmediarequest",JsonConvert.SerializeObject(uploadMediaRequest, Newtonsoft.Json.Formatting.None)),
                        new FormDataItem("webwx_data_ticket",httpClient.CookieContainer.GetCookies(new Uri(cookieRedirectUri))["webwx_data_ticket"].Value),
                        new FormDataItem("pass_ticket",passTicket),
                        new FormDataItem("filename",fileInfo.Name,buffer,readLength)
                    };
                string result = httpClient.PostMutipart(postUrl, dataList);
                response = JsonConvert.DeserializeObject<UploadMediaResponse>(result);
            }
            uploadMedia.Add(fileInfo.Name, response.MediaId);
            return response;
        }

        /// <summary>
        /// 同步通过好友认证
        /// </summary>
        /// <param name="info">sync中获得的申请信息</param>
        /// <returns></returns>
        public SimpleResponse VerifyUser(RecommendInfo info)
        {
            string verifyUserUrl = host + "/cgi-bin/mmwebwx-bin/webwxverifyuser?r=" + OtherUtils.GetJavaTimeStamp();
            VerifyUserRequest request = new VerifyUserRequest();
            request.BaseRequest = baseRequest;
            request.Opcode = VERIFYUSER_OPCODE.VERIFYUSER_OPCODE_VERIFYOK;
            request.SceneList = new List<int>() { (int)ADDSCENE_PF.ADDSCENE_PF_WEB };
            request.VerifyUserList = new List<Modal.Request.VerifyUser>() { new Modal.Request.VerifyUser { Value = info.UserName, VerifyUserTicket = info.Ticket } };
            request.skey = baseRequest.Skey;
            //反馈结果可以不理
            return httpClient.PostJson<SimpleResponse>(verifyUserUrl, request);
        }

        /// <summary>
        /// 获取上传文件时的MediaType
        /// </summary>
        /// <param name="ext">文件扩展名</param>
        /// <returns></returns>
        private string GetMediaType(string ext)
        {
            //貌似除了图片视频，其他都是doc
            switch (ext)
            {
                case ".jpg":
                    return "pic";
                case ".jpeg":
                    return "pic";
                case ".png":
                    return "pic";
                case ".mp4":
                    return "video";
                default:
                    return "doc";
            }
        }

        /// <summary>
        /// 同步修改备注
        /// 注意：多次调用该接口会被封
        /// </summary>
        /// <param name="remarkName">需要修改的备注名</param>
        /// <param name="userName">需要修改的联系人UserName</param>
        /// <returns></returns>
        public SimpleResponse RemarkName(string remarkName, string userName)
        {
            return OpLog(remarkName, userName, CmdIdType.MODREMARKNAME);
        }
        /// <summary>
        /// 同步顶置聊天
        /// 注意：多次调用该接口会被封
        /// </summary>
        /// <param name="remarkName">备注名，官方接口同时附带这个参数，我们也带上吧</param>
        /// <param name="userName">需要修改的联系人UserName</param>
        /// <returns></returns>
        public SimpleResponse TopContact(string remarkName, string userName)
        {
            return OpLog(remarkName, userName, CmdIdType.TOPCONTACT, 1);
        }

        /// <summary>
        /// 同步取消顶置消息
        /// </summary>
        /// <param name="remarkName">备注名，官方接口同时附带这个参数，我们也带上吧</param>
        /// <param name="userName">需要修改的联系人UserName</param>
        /// <returns></returns>
        public SimpleResponse UnTopContact(string remarkName, string userName)
        {
            return OpLog(remarkName, userName, CmdIdType.TOPCONTACT, 0);
        }

        private SimpleResponse OpLog(string remarkName, string userName, CmdIdType cmdIdType, int op = -1)
        {
            string opLogUrl = host + "/cgi-bin/mmwebwx-bin/webwxoplog";
            if (op != -1)
            {
                TopOpLogRequest request = new TopOpLogRequest()
                {
                    BaseRequest = baseRequest,
                    CmdId = cmdIdType,
                    OP = op,
                    RemarkName = remarkName,
                    UserName = userName
                };
                return httpClient.PostJson<SimpleResponse>(opLogUrl, request);
            }
            else
            {
                OpLogRequest request = new OpLogRequest()
                {
                    BaseRequest = baseRequest,
                    CmdId = cmdIdType,
                    RemarkName = remarkName,
                    UserName = userName
                };
                return httpClient.PostJson<SimpleResponse>(opLogUrl, request);
            }
        }

        /// <summary>
        /// 群里移除用户，用IsOwner判断自己是不是群主，否则没有权限
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="delName">用户UserName，英文,分割</param>
        /// <returns></returns>
        public UpdateChatRoomResponse RemoveChatRoomMember(string roomName, List<string> delNameList)
        {
            string delName = string.Join(",", delNameList);
            string url = host + "/cgi-bin/mmwebwx-bin/webwxupdatechatroom?fun=delmember&lang=zh_CN";
            DelMemberChatRoomRequest request = new DelMemberChatRoomRequest()
            {
                BaseRequest = baseRequest,
                ChatRoomName = roomName,
                DelMemberList = delName
            };
            return httpClient.PostJson<UpdateChatRoomResponse>(url, request);
        }

        /// <summary>
        /// 添加用户到群聊
        /// </summary>
        /// <param name="roomName">群UserName</param>
        /// <param name="addName">用户UserName，英文,分割</param>
        /// <returns></returns>
        public UpdateChatRoomResponse AddChatRoomMember(string roomName, List<string> addNameList)
        {
            string addName = string.Join(",", addNameList);
            string url = host + "/cgi-bin/mmwebwx-bin/webwxupdatechatroom?fun=addmember&lang=zh_CN";
            AddMemberChatRoomRequest request = new AddMemberChatRoomRequest()
            {
                BaseRequest = baseRequest,
                ChatRoomName = roomName,
                AddMemberList = addName
            };
            return httpClient.PostJson<UpdateChatRoomResponse>(url, request);
        }

        /// <summary>
        /// 创建群，调用完成，可以用返回的信息，通过GetBatchGetContact去获取群信息
        /// </summary>
        /// <param name="memberList">UserName的list</param>
        /// <returns></returns>
        public CreateChatRoomResponse CreateChatRoom(List<MemberItem> memberList)
        {
            string url = host + "/cgi-bin/mmwebwx-bin/webwxupdatechatroom?fun=addmember&lang=zh_CN";
            CreateChatRoomRequest request = new CreateChatRoomRequest()
            {
                BaseRequest = baseRequest,
                MemberList = memberList,
            };
            return httpClient.PostJson<CreateChatRoomResponse>(url, request);
        }
    }
}
