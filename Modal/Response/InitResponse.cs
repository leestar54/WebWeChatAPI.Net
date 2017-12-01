using Leestar54.WeChat.WebAPI.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal.Response
{
    public class InitResponse
    {
        /// <summary>
        /// BaseResponse
        /// </summary>
        public BaseResponse BaseResponse { get; set; }
        /// <summary>
        /// Count
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// ContactList
        /// </summary>
        public List<Contact> ContactList { get; set; }
        /// <summary>
        /// SyncKey
        /// </summary>
        public SyncKey SyncKey { get; set; }
        /// <summary>
        /// User
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// filehelper,@2f4024df37e9ff42d2d095354916185e,@29687b9dc60c7d44424b6a3d9b478e42,
        /// </summary>
        public string ChatSet { get; set; }
        /// <summary>
        /// @crypt_372b266_2f6f7cb1612ee9b9758ce0e39baa3ccb
        /// </summary>
        public string SKey { get; set; }
        /// <summary>
        /// ClientVersion
        /// </summary>
        public int ClientVersion { get; set; }
        /// <summary>
        /// SystemTime
        /// </summary>
        public int SystemTime { get; set; }
        /// <summary>
        /// GrayScale
        /// </summary>
        public int GrayScale { get; set; }
        /// <summary>
        /// InviteStartCount
        /// </summary>
        public int InviteStartCount { get; set; }
        /// <summary>
        /// MPSubscribeMsgCount
        /// </summary>
        public int MPSubscribeMsgCount { get; set; }
        /// <summary>
        /// MPSubscribeMsgList
        /// </summary>
        public List<MPSubscribeMsg> MPSubscribeMsgList { get; set; }
        /// <summary>
        /// ClickReportInterval
        /// </summary>
        public int ClickReportInterval { get; set; }
    }

}
