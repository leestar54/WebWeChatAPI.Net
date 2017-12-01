using Leestar54.WeChat.WebAPI.Modal.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal
{
    /// <summary>
    /// 请求好友时的信息
    /// </summary>
    public class RecommendInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 请求人昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// QQNum
        /// </summary>
        public int QQNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 加好友请求消息
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Signature { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// Scene
        /// </summary>
        public int Scene { get; set; }
        /// <summary>
        /// VerifyFlag
        /// </summary>
        public int VerifyFlag { get; set; }
        /// <summary>
        /// AttrStatus
        /// </summary>
        public Int64 AttrStatus { get; set; }
        /// <summary>
        /// Sex
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// VerifyUser中用于接受请求凭证
        /// </summary>
        public string Ticket { get; set; }
        /// <summary>
        /// 操作码
        /// </summary>
        public VERIFYUSER_OPCODE OpCode { get; set; }
    }

}
