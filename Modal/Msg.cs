using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal
{
    public enum MSGTYPE
    {
        MSGTYPE_TEXT = 1,
        MSGTYPE_IMAGE = 3,
        MSGTYPE_DOC = 6,
        MSGTYPE_VOICE = 34,
        MSGTYPE_VIDEO = 43,
        MSGTYPE_MICROVIDEO = 62,
        MSGTYPE_EMOTICON = 47,
        MSGTYPE_APP = 49,
        MSGTYPE_VOIPMSG = 50,
        MSGTYPE_VOIPNOTIFY = 52,
        MSGTYPE_VOIPINVITE = 53,
        MSGTYPE_LOCATION = 48,
        MSGTYPE_STATUSNOTIFY = 51,
        MSGTYPE_SYSNOTICE = 9999,
        MSGTYPE_POSSIBLEFRIEND_MSG = 40,
        MSGTYPE_VERIFYMSG = 37,
        MSGTYPE_SHARECARD = 42,
        MSGTYPE_SYS = 10000,
        MSGTYPE_RECALLED = 10002,
    }

    /// <summary>
    /// 文本消息
    /// </summary>
    public class Msg
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public MSGTYPE Type { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 发送人
        /// </summary>
        public string FromUserName { get; set; }
        /// <summary>
        /// 接收人
        /// </summary>
        public string ToUserName { get; set; }
        /// <summary>
        /// 当前时间戳
        /// </summary>
        public string LocalID { get; set; }
        /// <summary>
        /// 当前时间戳
        /// </summary>
        public string ClientMsgId { get; set; }
    }

    /// <summary>
    /// 媒体消息
    /// </summary>
    public class MediaMsg : Msg
    {
        /// <summary>
        /// 上传完成文件之后，获得的文件id
        /// </summary>
        public string MediaId { get; set; }
    }
}
