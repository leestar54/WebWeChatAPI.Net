using Leestar54.WeChat.WebAPI;
using Leestar54.WeChat.WebAPI.Modal;
using Leestar54.WeChat.WebAPI.Modal.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{


    class Program
    {
        private static Client client;
        private static Dictionary<string, Contact> contactDict = new Dictionary<string, Contact>();
        private static QrCodeForm qrForm;
        static void Main(string[] args)
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            client = new Client();
            qrForm = new QrCodeForm();

            client.ExceptionCatched += Client_ExceptionCatched; ;
            client.GetLoginQrCodeComplete += Client_GetLoginQrCodeComplete; ;
            client.CheckScanComplete += Client_CheckScanComplete; ;
            client.LoginComplete += Client_LoginComplete; ;
            client.BatchGetContactComplete += Client_BatchGetContactComplete; ;
            client.GetContactComplete += Client_GetContactComplete; ;
            client.MPSubscribeMsgListComplete += Client_MPSubscribeMsgListComplete; ;
            client.LogoutComplete += Client_LogoutComplete; ;
            client.ReceiveMsg += Client_ReceiveMsg; ;
            client.DelContactListComplete += Client_DelContactListComplete; ;
            client.ModContactListComplete += Client_ModContactListComplete;
            Console.WriteLine("小助手启动");
            client.Start();
            qrForm.ShowDialog();

            Console.ReadLine();
            client.Close();
            Console.ReadLine();
            client.Logout();

            //获取群成员详情，需要我们主动调用，一般用不到，因为群里已经包含Member基本信息。
            //Contact chatRoom = contactDict["群UserName"];
            //string listStr = string.Join(",", chatRoom.MemberList);
            //client.GetBatchGetContactAsync(listStr, chatRoom.UserName);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

        private static void Client_ModContactListComplete(object sender, TEventArgs<List<ModContactItem>> e)
        {
            Console.WriteLine("接收修改联系人信息");
        }

        private static void Client_DelContactListComplete(object sender, TEventArgs<List<DelContactItem>> e)
        {
            Console.WriteLine("接收删除联系人信息");
        }

        private static void Client_ReceiveMsg(object sender, TEventArgs<List<AddMsg>> e)
        {
            Console.WriteLine("接收新消息");
            foreach (var item in e.Result)
            {
                switch (item.MsgType)
                {
                    case MsgType.MM_DATA_TEXT:
                        if (contactDict.Keys.Contains(item.FromUserName))
                        {
                            Console.WriteLine(contactDict[item.FromUserName].NickName + ":" + item.Content);
                        }
                        else
                        {
                            //不包含（一般为群）则需要我们主动拉取信息
                            client.GetBatchGetContactAsync(item.FromUserName);
                        }

                        //自动回复
                        if (item.Content == "666666")
                        {
                            client.SendMsgAsync("双击666！", item.FromUserName);
                        }
                        break;
                    case MsgType.MM_DATA_HTML:
                        break;
                    case MsgType.MM_DATA_IMG:
                        break;
                    case MsgType.MM_DATA_PRIVATEMSG_TEXT:
                        break;
                    case MsgType.MM_DATA_PRIVATEMSG_HTML:
                        break;
                    case MsgType.MM_DATA_PRIVATEMSG_IMG:
                        break;
                    case MsgType.MM_DATA_VOICEMSG:
                        break;
                    case MsgType.MM_DATA_PUSHMAIL:
                        break;
                    case MsgType.MM_DATA_QMSG:
                        break;
                    case MsgType.MM_DATA_VERIFYMSG:
                        //自动加好友
                        client.VerifyUser(item.RecommendInfo);
                        break;
                    case MsgType.MM_DATA_PUSHSYSTEMMSG:
                        break;
                    case MsgType.MM_DATA_QQLIXIANMSG_IMG:
                        break;
                    case MsgType.MM_DATA_POSSIBLEFRIEND_MSG:
                        break;
                    case MsgType.MM_DATA_SHARECARD:
                        break;
                    case MsgType.MM_DATA_VIDEO:
                        break;
                    case MsgType.MM_DATA_VIDEO_IPHONE_EXPORT:
                        break;
                    case MsgType.MM_DATA_EMOJI:
                        break;
                    case MsgType.MM_DATA_LOCATION:
                        break;
                    case MsgType.MM_DATA_APPMSG:
                        break;
                    case MsgType.MM_DATA_VOIPMSG:
                        break;
                    case MsgType.MM_DATA_STATUSNOTIFY:
                        switch (item.StatusNotifyCode)
                        {
                            case StatusNotifyCode.StatusNotifyCode_READED:
                                break;
                            case StatusNotifyCode.StatusNotifyCode_ENTER_SESSION:
                                break;
                            case StatusNotifyCode.StatusNotifyCode_INITED:
                                break;
                            case StatusNotifyCode.StatusNotifyCode_SYNC_CONV:
                                //初始化的时候第一次sync会返回最近聊天的列表
                                client.GetBatchGetContactAsync(item.StatusNotifyUserName);
                                break;
                            case StatusNotifyCode.StatusNotifyCode_QUIT_SESSION:
                                break;
                            default:
                                break;
                        }
                        break;
                    case MsgType.MM_DATA_VOIPNOTIFY:
                        break;
                    case MsgType.MM_DATA_VOIPINVITE:
                        break;
                    case MsgType.MM_DATA_MICROVIDEO:
                        break;
                    case MsgType.MM_DATA_SYSNOTICE:
                        break;
                    case MsgType.MM_DATA_SYS:
                        break;
                    case MsgType.MM_DATA_RECALLED:
                        break;
                    default:
                        break;
                }
            }
        }

        private static void Client_LogoutComplete(object sender, TEventArgs<User> e)
        {
            Console.WriteLine("已登出");
            Application.Exit();
        }

        private static void Client_MPSubscribeMsgListComplete(object sender, TEventArgs<List<MPSubscribeMsg>> e)
        {
            Console.WriteLine("获取公众号文章，总数：" + e.Result.Count);
        }

        private static void Client_GetContactComplete(object sender, TEventArgs<List<Contact>> e)
        {
            Console.WriteLine("获取联系人列表，总数：" + e.Result.Count);
            foreach (var item in e.Result)
            {
                if (!contactDict.Keys.Contains(item.UserName))
                {
                    contactDict.Add(item.UserName, item);
                }

                //联系人列表中包含联系人，公众号，可以通过参数做区分
                if (item.VerifyFlag != 0)
                {
                    //个人号
                }
                else
                {
                    //公众号
                }
            }
        }

        private static void Client_BatchGetContactComplete(object sender, TEventArgs<List<Contact>> e)
        {
            Console.WriteLine("拉取联系人信息，总数：" + e.Result.Count);
            foreach (var item in e.Result)
            {
                //群成员详细信息也会回调到此处,可以这么判断
                if (item.EncryChatRoomId != "0" && !item.UserName.StartsWith("@@"))
                {
                    Console.WriteLine("接受到群成员详情");
                }
                else if (!contactDict.Keys.Contains(item.UserName))
                {
                    contactDict.Add(item.UserName, item);
                }
            }
        }

        private static void Client_LoginComplete(object sender, TEventArgs<User> e)
        {
            Console.WriteLine("登陆成功：" + e.Result.NickName);
        }

        private static void Client_CheckScanComplete(object sender, TEventArgs<System.Drawing.Image> e)
        {
            Console.WriteLine("用户已扫码");
            qrForm.SetPic(e.Result);
        }

        private static void Client_GetLoginQrCodeComplete(object sender, TEventArgs<System.Drawing.Image> e)
        {
            Console.WriteLine("已获取登陆二维码");
            qrForm.SetPic(e.Result);
        }

        private static void Client_ExceptionCatched(object sender, TEventArgs<Exception> e)
        {
            Console.WriteLine("异常：" + e.Result.ToString());
        }
    }
}
