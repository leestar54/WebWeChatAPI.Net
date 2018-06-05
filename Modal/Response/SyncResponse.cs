using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal.Response
{
    public class UserName
    {
        /// <summary>
        /// 
        /// </summary>
        public string Buff { get; set; }
    }

    public class NickName
    {
        /// <summary>
        /// 
        /// </summary>
        public string Buff { get; set; }
    }

    public class BindEmail
    {
        /// <summary>
        /// 
        /// </summary>
        public string Buff { get; set; }
    }

    public class BindMobile
    {
        /// <summary>
        /// 
        /// </summary>
        public string Buff { get; set; }
    }

    public class Profile
    {
        /// <summary>
        /// BitFlag
        /// </summary>
        public int BitFlag { get; set; }
        /// <summary>
        /// UserName
        /// </summary>
        public UserName UserName { get; set; }
        /// <summary>
        /// NickName
        /// </summary>
        public NickName NickName { get; set; }
        /// <summary>
        /// BindUin
        /// </summary>
        public long BindUin { get; set; }
        /// <summary>
        /// BindEmail
        /// </summary>
        public BindEmail BindEmail { get; set; }
        /// <summary>
        /// BindMobile
        /// </summary>
        public BindMobile BindMobile { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public long Status { get; set; }
        /// <summary>
        /// Sex
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// PersonalCard
        /// </summary>
        public int PersonalCard { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// HeadImgUpdateFlag
        /// </summary>
        public int HeadImgUpdateFlag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HeadImgUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Signature { get; set; }
    }
    public class SyncCheckKey
    {
        /// <summary>
        /// Count
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// List
        /// </summary>
        public List<KeyValPair> List { get; set; }
    }

    public class DelContactItem
    {
        /// <summary>
        /// @@6b8b582ff695fcd28645382724c328bd768015958a7e0fadaf4e87504da52803
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// ContactFlag
        /// </summary>
        public int ContactFlag { get; set; }
    }

    public class SyncResponse
    {
        /// <summary>
        /// BaseResponse
        /// </summary>
        public BaseResponse BaseResponse { get; set; }
        /// <summary>
        /// AddMsgCount
        /// </summary>
        public int AddMsgCount { get; set; }
        /// <summary>
        /// AddMsgList
        /// </summary>
        public List<AddMsg> AddMsgList { get; set; }
        /// <summary>
        /// ModContactCount
        /// </summary>
        public int ModContactCount { get; set; }
        /// <summary>
        /// ModContactList
        /// </summary>
        public List<Contact> ModContactList { get; set; }
        /// <summary>
        /// DelContactCount
        /// </summary>
        public int DelContactCount { get; set; }
        /// <summary>
        /// DelContactList
        /// </summary>
        public List<DelContactItem> DelContactList { get; set; }
        /// <summary>
        /// ModChatRoomMemberCount
        /// </summary>
        public int ModChatRoomMemberCount { get; set; }
        /// <summary>
        /// ModChatRoomMemberList
        /// </summary>
        public List<string> ModChatRoomMemberList { get; set; }
        /// <summary>
        /// Profile
        /// </summary>
        public Profile Profile { get; set; }
        /// <summary>
        /// ContinueFlag
        /// </summary>
        public int ContinueFlag { get; set; }
        /// <summary>
        /// SyncKey
        /// </summary>
        public SyncKey SyncKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SKey { get; set; }
        /// <summary>
        /// SyncCheckKey
        /// </summary>
        public SyncCheckKey SyncCheckKey { get; set; }
    }

}
