using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal.Request
{

    class UpdateChatRoomRequest
    {
        /// <summary>
        /// @@8404204dd2d683452eb81bcbbf383d323d6e01dc4a7cc7a7ae511b6e94ff5237
        /// </summary>
        public string ChatRoomName { get; set; }
        /// <summary>
        /// BaseRequest
        /// </summary>
        public BaseRequest BaseRequest { get; set; }
    }
    class AddMemberChatRoomRequest : UpdateChatRoomRequest
    {
        /// <summary>
        /// @8e9f05c28b7750c36c487c8ba7ca3781
        /// </summary>
        public string AddMemberList { get; set; }
    }

    class DelMemberChatRoomRequest : UpdateChatRoomRequest
    {
        /// <summary>
        /// @8e9f05c28b7750c36c487c8ba7ca3781
        /// </summary>
        public string DelMemberList { get; set; }
    }
}
