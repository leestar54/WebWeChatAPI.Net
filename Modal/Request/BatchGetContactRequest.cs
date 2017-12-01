using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal.Request
{
    public class ChatRoom
    {
        /// <summary>
        /// @@7f06ff68be77658b8635dfcd6dc0fb81d31c69b9aeb626d67a2105d85903a2c5
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EncryChatRoomId { get; set; }
    }

    public class BatchGetContactRequest
    {
        /// <summary>
        /// BaseRequest
        /// </summary>
        public BaseRequest BaseRequest { get; set; }
        /// <summary>
        /// Count
        /// </summary>
        public int Count { get { return List.Count; } }
        /// <summary>
        /// List
        /// </summary>
        public List<ChatRoom> List { get; set; }
    }

}
