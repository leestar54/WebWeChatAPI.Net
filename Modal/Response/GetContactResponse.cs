using Leestar54.WeChat.WebAPI.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal.Response
{
    public class GetContactResponse
    {
        /// <summary>
        /// BaseResponse
        /// </summary>
        public BaseResponse BaseResponse { get; set; }
        /// <summary>
        /// MemberCount
        /// </summary>
        public int MemberCount { get; set; }
        /// <summary>
        /// MemberList
        /// </summary>
        public List<Contact> MemberList { get; set; }
        /// <summary>
        /// 这个参数用于连续获取用户，超过4000之后一次是取不完的，当seq为0的时候，才结束，否则继续带上参数获取。
        /// </summary>
        public int Seq { get; set; }
    }
}
