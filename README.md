# WebWeChat.Net
基于.Net平台C#的微信网页版API

注：交互过程可以使用fiddler工具分析

## 近期更新

* 新增已知问题过滤，包括：千人账号无法获取好友列表，新注册账号无法登陆网页版
* 优化稳定性，开发环境升级.Net至4.6。因为win7系统偶尔会出现The request was aborted: Could not create SSL/TLS secure channel，而win10没有问题。经研究是.Net自身对于tls协议支持上的bug，建议将系统补丁打至最新，并且framework升级至最新4.7.1，framework下载地址：https://www.microsoft.com/net/download/thank-you/net471
* 实现API基础功能


## 开发环境

vs2015+.net4.6.1 framework

## 依赖项

json.net

## Feature

* 最小依赖，使用简单
* 支持同步、基于事件回调的异步方法
* 对象间隔离，可以实例化无数客户端

## 简单使用

具体内容见源码，此处仅简单说明

```c#
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
}
```

## 方法说明

```c#
/// <summary>
/// 异步发送文字消息
/// </summary>
/// <param name="msg">消息</param>
/// <param name="toUserName">发送人UserName</param>
public void SendMsgAsync(string msg, string toUserName)

/// <summary>
/// 同步发送文字消息
/// </summary>
/// <param name="msg">文字</param>
/// <param name="toUserName">发送人UserName</param>
/// <returns></returns>
public SendMsgResponse SendMsg(string msg, string toUserName)

/// <summary>
/// 异步发送文件
/// </summary>
/// <param name="fileInfo">文件信息</param>
/// <param name="toUserName">发送人UserName</param>
public void SendMsgAsync(FileInfo fileInfo, string toUserName)

/// <summary>
/// 同步发送文件，自动分块上传，文件较大可能会卡住进程，建议异步发送
/// </summary>
/// <param name="fileInfo">文件信息</param>
/// <param name="toUserName">发送人UserName</param>
/// <returns></returns>
public SendMsgResponse SendMsg(FileInfo fileInfo, string toUserName)

/// <summary>
/// 获取头像，因为请求的时候需要带Cookie等相关参数，所以直接用新的http请求不行，务必使用客户端API来获取
/// </summary>
/// <param name="url">头像地址，例如/cgi-bin/mmwebwx-bin/webwxgeticon?seq=0&username=filehelper&skey=@crypt_372b266_540d016177e861740ee84fec697a3b01 </param>
/// <param name="action">委托Action</param>
/// <returns></returns>
public void GetIconAsync(string url, Action<Image> action)

/// <summary>
/// 同步上传文件
/// </summary>
/// <param name="fileInfo">文件信息</param>
/// <returns></returns>
public UploadMediaResponse UploadFile(FileInfo fileInfo)

/// <summary>
/// 同步修改备注
/// 注意：多次调用该接口会被封
/// </summary>
/// <param name="remarkName">需要修改的备注名</param>
/// <param name="userName">需要修改的联系人UserName</param>
/// <returns></returns>
public SimpleResponse RemarkName(string remarkName, string userName)

/// <summary>
/// 同步通过好友认证
/// </summary>
/// <param name="info">sync中获得的申请信息</param>
/// <returns></returns>
public SimpleResponse VerifyUser(RecommendInfo info)

/// <summary>
/// 同步顶置聊天
/// 注意：多次调用该接口会被封
/// </summary>
/// <param name="remarkName">备注名，官方接口同时附带这个参数，我们也带上吧</param>
/// <param name="userName">需要修改的联系人UserName</param>
/// <returns></returns>
public SimpleResponse TopContact(string remarkName, string userName)

/// <summary>
/// 同步取消顶置消息
/// </summary>
/// <param name="remarkName">备注名，官方接口同时附带这个参数，我们也带上吧</param>
/// <param name="userName">需要修改的联系人UserName</param>
/// <returns></returns>
public SimpleResponse UnTopContact(string remarkName, string userName)

/// <summary>
/// 群里移除用户，用IsOwner判断自己是不是群主，否则没有权限
/// </summary>
/// <param name="roomName"></param>
/// <param name="delName">用户UserName，英文,分割</param>
/// <returns></returns>
public UpdateChatRoomResponse RemoveChatRoomMember(string roomName, List<string> delNameList)

/// <summary>
/// 添加用户到群聊
/// </summary>
/// <param name="roomName">群UserName</param>
/// <param name="addName">用户UserName，英文,分割</param>
/// <returns></returns>
public UpdateChatRoomResponse AddChatRoomMember(string roomName, List<string> addNameList)

/// <summary>
/// 创建群，调用完成，可以用返回的信息，通过GetBatchGetContact去获取群信息
/// </summary>
/// <param name="memberList">UserName的list</param>
/// <returns></returns>
public CreateChatRoomResponse CreateChatRoom(List<MemberItem> memberList)
```



## 参考

* [liuwons/wxBot](https://github.com/liuwons/wxBot)

* [Urinx/WeixinBot](https://github.com/Urinx/WeixinBot)

* [lbbniu/WebWechat](https://github.com/lbbniu/WebWechat)



