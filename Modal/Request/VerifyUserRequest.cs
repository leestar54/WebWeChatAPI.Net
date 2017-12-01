using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal.Request
{
    public enum VERIFYUSER_OPCODE
    {
        VERIFYUSER_OPCODE_ADDCONTACT = 1,
        VERIFYUSER_OPCODE_SENDREQUEST = 2,
        VERIFYUSER_OPCODE_VERIFYOK = 3,
        VERIFYUSER_OPCODE_VERIFYREJECT = 4,
        VERIFYUSER_OPCODE_SENDERREPLY = 5,
        VERIFYUSER_OPCODE_RECVERREPLY = 6,
    }

    public enum ADDSCENE_PF
    {
        ADDSCENE_PF_QQ = 4,
        ADDSCENE_PF_EMAIL = 5,
        ADDSCENE_PF_CONTACT = 6,
        ADDSCENE_PF_WEIXIN = 7,
        ADDSCENE_PF_GROUP = 8,
        ADDSCENE_PF_UNKNOWN = 9,
        ADDSCENE_PF_MOBILE = 10,
        ADDSCENE_PF_WEB = 33,
    }

    public class VerifyUser
    {
        /// <summary>
        /// @831657f3122f1548a8fbf50a558c4e53
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// v2_e7e35e3a4a319226ddd30993c4ac722991d568c574ca2bd3be62548710b1d1572591de1cf96dbe0656ce9930038af3fffaeae72d11175c9555440ddd2288749c@stranger
        /// </summary>
        public string VerifyUserTicket { get; set; }
    }

    public class VerifyUserRequest
    {
        /// <summary>
        /// BaseRequest
        /// </summary>
        public BaseRequest BaseRequest { get; set; }
        /// <summary>
        /// 操作码
        /// </summary>
        public VERIFYUSER_OPCODE Opcode { get; set; }
        /// <summary>
        /// VerifyUserListSize
        /// </summary>
        public int VerifyUserListSize
        {
            get
            {
                return VerifyUserList.Count;
            }
        }
        /// <summary>
        /// VerifyUserList
        /// </summary>
        public List<VerifyUser> VerifyUserList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string VerifyContent { get; set; }
        /// <summary>
        /// SceneListCount
        /// </summary>
        public int SceneListCount
        {
            get
            {
                return SceneList.Count;
            }
        }
        /// <summary>
        /// 场景标识
        /// </summary>
        public List<int> SceneList { get; set; }
        /// <summary>
        /// @crypt_372b266_9f05c7b43293f66534700c288956ea7c
        /// </summary>
        public string skey { get; set; }
    }
}
