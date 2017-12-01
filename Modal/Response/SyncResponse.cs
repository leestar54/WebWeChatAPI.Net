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
        public int BindUin { get; set; }
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
        public int Status { get; set; }
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
    public class ModContactItem
    {
        /// <summary>
        /// @@039c88aeaa8efaaea6a97cca56fb6a30f5a72bfeb6f810939c1a6904e24a3eb9
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// Sex
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// HeadImgUpdateFlag
        /// </summary>
        public int HeadImgUpdateFlag { get; set; }
        /// <summary>
        /// ContactType
        /// </summary>
        public int ContactType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// @1a4f170f4ab3053a42cfa7e1f5ec9b74e556a38fef594d37d87a8c2dee52a6d3
        /// </summary>
        public string ChatRoomOwner { get; set; }
        /// <summary>
        /// /cgi-bin/mmwebwx-bin/webwxgetheadimg?seq=0&username=@@039c88aeaa8efaaea6a97cca56fb6a30f5a72bfeb6f810939c1a6904e24a3eb9&skey=@crypt_372b266_540d016177e861740ee84fec697a3b01
        /// </summary>
        public string HeadImgUrl { get; set; }
        /// <summary>
        /// ContactFlag
        /// </summary>
        public int ContactFlag { get; set; }
        /// <summary>
        /// MemberCount
        /// </summary>
        public int MemberCount { get; set; }
        /// <summary>
        /// MemberList
        /// </summary>
        public List<Member> MemberList { get; set; }
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
        public List<ModContactItem> ModContactList { get; set; }
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
