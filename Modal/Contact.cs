using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal
{
    /// <summary>
    /// BatchGetContact、GetContact获取到的联系人信息，包括公众号，个人号，群
    /// </summary>
    public class Contact : User
    {
        /// <summary>
        /// 成员数，群用
        /// </summary>
        public int MemberCount { get; set; }
        /// <summary>
        /// 成员信息，群用
        /// </summary>
        public List<Member> MemberList { get; set; }

        private Dictionary<string, Member> memberDict = null;

        /// <summary>
        /// 成员字典，方便查询
        /// </summary>
        public Dictionary<string, Member> MemberDict
        {
            get
            {
                if (memberDict == null)
                {
                    memberDict = MemberList.ToDictionary(key => key.UserName, value => value);
                }
                return memberDict;
            }
        }

        /// <summary>
        /// OwnerUin
        /// </summary>
        public int OwnerUin { get; set; }
        /// <summary>
        /// 是否开启免打扰，群用
        /// CHATROOM_NOTIFY_OPEN: 1,
        /// CHATROOM_NOTIFY_CLOSE: 0
        /// </summary>
        public int Statues { get; set; }
        /// <summary>
        /// AttrStatus
        /// </summary>
        public Int64 AttrStatus { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 基本都是空
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// 基本都是0
        /// </summary>
        public int UniFriend { get; set; }
        /// <summary>
        /// 基本都是空
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 群id，一般为0，请用EncryChatRoomId
        /// </summary>
        public int ChatRoomId { get; set; }
        /// <summary>
        /// 键值，可能用于搜索
        /// </summary>
        public string KeyWord { get; set; }
        /// <summary>
        /// 加密的群ID
        /// </summary>
        public string EncryChatRoomId { get; set; }
        /// <summary>
        /// 是否是群主，1是，0不是，群用
        /// </summary>
        public int IsOwner { get; set; }

        /// <summary>
        /// 是否是群聊
        /// </summary>
        /// <returns></returns>
        public bool IsChatRoom()
        {
            return EncryChatRoomId != "0";
        }
    }

}
