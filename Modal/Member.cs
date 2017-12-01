using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal
{
    /// <summary>
    /// 群成员信息
    /// </summary>
    public class Member
    {
        /// <summary>
        /// 都是0
        /// </summary>
        public int Uin { get; set; }
        /// <summary>
        /// 群内的UserName
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// AttrStatus
        /// </summary>
        public Int64 AttrStatus { get; set; }
        /// <summary>
        /// 简拼
        /// </summary>
        public string PYInitial { get; set; }
        /// <summary>
        /// 全拼
        /// </summary>
        public string PYQuanPin { get; set; }
        /// <summary>
        /// 备注简拼
        /// </summary>
        public string RemarkPYInitial { get; set; }
        /// <summary>
        /// 备注全拼
        /// </summary>
        public string RemarkPYQuanPin { get; set; }
        /// <summary>
        /// 基本为0
        /// </summary>
        public int MemberStatus { get; set; }
        /// <summary>
        /// 基本为空
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 键值，可能用于搜索
        /// </summary>
        public string KeyWord { get; set; }
    }
}
