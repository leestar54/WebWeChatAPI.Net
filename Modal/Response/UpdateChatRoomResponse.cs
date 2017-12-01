using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal.Response
{
    public class UpdateChatRoomResponse
    {    /// <summary>
         /// BaseResponse
         /// </summary>
        public BaseResponse BaseResponse { get; set; }
        /// <summary>
        /// MemberCount
        /// </summary>
        public int MemberCount { get { return MemberList.Count; } }
        /// <summary>
        /// MemberList
        /// </summary>
        public List<Member> MemberList { get; set; }
    }
}
