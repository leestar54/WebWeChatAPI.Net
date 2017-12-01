using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal.Response
{
    public class CreateChatRoomResponse
    {/// <summary>
     /// BaseResponse
     /// </summary>
        public BaseResponse BaseResponse { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PYInitial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string QuanPin { get; set; }
        /// <summary>
        /// MemberCount
        /// </summary>
        public int MemberCount { get; set; }
        /// <summary>
        /// MemberList
        /// </summary>
        public List<Member> MemberList { get; set; }
        /// <summary>
        /// @@e88c4e1e0a60e6bc712953fe6e3a711955e8f5fa24b3f7374d98550e0dad2147
        /// </summary>
        public string ChatRoomName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BlackList { get; set; }
    }
}
