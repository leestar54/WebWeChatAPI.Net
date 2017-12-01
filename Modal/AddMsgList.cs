using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal
{
    public enum MsgType
    {
        MM_DATA_TEXT = 1,
        MM_DATA_HTML = 2,
        MM_DATA_IMG = 3,
        MM_DATA_PRIVATEMSG_TEXT = 11,
        MM_DATA_PRIVATEMSG_HTML = 12,
        MM_DATA_PRIVATEMSG_IMG = 13,
        MM_DATA_VOICEMSG = 34,
        MM_DATA_PUSHMAIL = 35,
        MM_DATA_QMSG = 36,
        MM_DATA_VERIFYMSG = 37,
        MM_DATA_PUSHSYSTEMMSG = 38,
        MM_DATA_QQLIXIANMSG_IMG = 39,
        MM_DATA_POSSIBLEFRIEND_MSG = 40,
        MM_DATA_SHARECARD = 42,
        MM_DATA_VIDEO = 43,
        MM_DATA_VIDEO_IPHONE_EXPORT = 44,
        MM_DATA_EMOJI = 47,
        MM_DATA_LOCATION = 48,
        MM_DATA_APPMSG = 49,
        MM_DATA_VOIPMSG = 50,
        MM_DATA_STATUSNOTIFY = 51,
        MM_DATA_VOIPNOTIFY = 52,
        MM_DATA_VOIPINVITE = 53,
        MM_DATA_MICROVIDEO = 62,
        MM_DATA_SYSNOTICE = 9999,
        MM_DATA_SYS = 10000,
        MM_DATA_RECALLED = 10002,
    }

    public enum StatusNotifyCode
    {
        StatusNotifyCode_READED = 1,
        StatusNotifyCode_ENTER_SESSION = 2,
        StatusNotifyCode_INITED = 3,
        StatusNotifyCode_SYNC_CONV = 4,
        StatusNotifyCode_QUIT_SESSION = 5,
    }

    public class AddMsg
    {
        /// <summary>
        /// 8823881319420351252
        /// </summary>
        public string MsgId { get; set; }
        /// <summary>
        /// @e985b6acfda8f6d6059b82b7fdfda89c4ab93d943aefd3508eca54462359b566
        /// </summary>
        public string FromUserName { get; set; }
        /// <summary>
        /// @56d16d9903cdd1e9f42181fd931d2236
        /// </summary>
        public string ToUserName { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public MsgType MsgType { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// ImgStatus
        /// </summary>
        public int ImgStatus { get; set; }
        /// <summary>
        /// CreateTime
        /// </summary>
        public int CreateTime { get; set; }
        /// <summary>
        /// VoiceLength
        /// </summary>
        public int VoiceLength { get; set; }
        /// <summary>
        /// PlayLength
        /// </summary>
        public int PlayLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FileSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MediaId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// AppMsgType
        /// </summary>
        public int AppMsgType { get; set; }
        /// <summary>
        /// 提示消息类别
        /// </summary>
        public StatusNotifyCode StatusNotifyCode { get; set; }
        /// <summary>
        /// 一个也可能是多个，需要获取信息，直接GetBatchGetContactAsync传入参数即可。
        /// @56d16d9903cdd1e9f42181fd931d2236
        /// </summary>
        public string StatusNotifyUserName { get; set; }
        /// <summary>
        /// RecommendInfo
        /// </summary>
        public RecommendInfo RecommendInfo { get; set; }
        /// <summary>
        /// ForwardFlag
        /// </summary>
        public int ForwardFlag { get; set; }
        /// <summary>
        /// AppInfo
        /// </summary>
        public AppInfo AppInfo { get; set; }
        /// <summary>
        /// HasProductId
        /// </summary>
        public int HasProductId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Ticket { get; set; }
        /// <summary>
        /// ImgHeight
        /// </summary>
        public int ImgHeight { get; set; }
        /// <summary>
        /// ImgWidth
        /// </summary>
        public int ImgWidth { get; set; }
        /// <summary>
        /// SubMsgType
        /// </summary>
        public int SubMsgType { get; set; }
        /// <summary>
        /// NewMsgId
        /// </summary>
        public Int64 NewMsgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OriContent { get; set; }
    }
}
