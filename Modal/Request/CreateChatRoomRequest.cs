using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal.Request
{
    public class MemberItem
    {
        /// <summary>
        /// @4ed575f6e610b3f192dec91e495c0b96469c72ac0af22a997afbf3785dc4af4e
        /// </summary>
        public string UserName { get; set; }
    }
    class CreateChatRoomRequest
    {
        /// <summary>
        /// MemberCount
        /// </summary>
        public int MemberCount { get { return MemberList.Count; } }
        /// <summary>
        /// MemberList
        /// </summary>
        public List<MemberItem> MemberList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// BaseRequest
        /// </summary>
        public BaseRequest BaseRequest { get; set; }
    }
}
