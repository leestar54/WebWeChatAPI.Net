using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal
{
    public enum CONTACTFLAG
    {
        CONTACTFLAG_CONTACT = 1,
        CONTACTFLAG_CHATCONTACT = 2,
        CONTACTFLAG_CHATROOMCONTACT = 4,
        CONTACTFLAG_BLACKLISTCONTACT = 8,
        CONTACTFLAG_DOMAINCONTACT = 16,
        CONTACTFLAG_HIDECONTACT = 32,
        CONTACTFLAG_FAVOURCONTACT = 64,
        CONTACTFLAG_3RDAPPCONTACT = 128,
        CONTACTFLAG_SNSBLACKLISTCONTACT = 256,
        CONTACTFLAG_NOTIFYCLOSECONTACT = 512,
        CONTACTFLAG_TOPCONTACT = 2048,
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    public class User
    {
        /// <summary>
        /// 个人号唯一识别码，不会变
        /// </summary>
        public string Uin { get; set; }
        /// <summary>
        /// 每次登陆都会变
        /// 个人号
        /// 以@开头，例如：@xxx
        /// 
        /// 群聊
        /// 以@@开头，例如：@@xxx
        /// 
        /// 公众号
        /// 以@开头，还要配合VerifyFlag字段判判断
        /// 
        /// 特殊账号
        /// 像文件传输助手之类的账号，有特殊的ID，目前已知的有： filehelper, newsapp, fmessage, weibo, qqmail, fmessage, tmessage, qmessage, qqsync, floatbottle, lbsapp, shakeapp, medianote, qqfriend, readerapp, blogapp, facebookapp, masssendapp, meishiapp, feedsapp, voip, blogappweixin, weixin, brandsessionholder, weixinreminder, officialaccounts, notification_messages, wxitil, userexperience_alarm, notification_messages
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 昵称，群若未命名，则为空
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 头像图片链接地址，务必用client的接口去读取。
        /// </summary>
        public string HeadImgUrl { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string RemarkName { get; set; }
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
        /// HideInputBarFlag
        /// </summary>
        public int HideInputBarFlag { get; set; }
        /// <summary>
        /// 是否是标星朋友
        /// </summary>
        public int StarFriend { get; set; }
        /// <summary>
        ///0公众号, 1男，2女
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string Signature { get; set; }
        /// <summary>
        /// AppAccountFlag
        /// </summary>
        public int AppAccountFlag { get; set; }
        /// <summary>
        /// 用来判断账号类型，以@开头
        /// 个人号 0
        /// 一般公众号：8
        /// 认证的公众号：24
        /// 机构过期 29
        /// 微信官方账号微信团队：56
        /// </summary>
        public int VerifyFlag { get; set; }
        /// <summary>
        /// 会话标识
        /// </summary>
        public CONTACTFLAG ContactFlag { get; set; }
        /// <summary>
        /// WebWxPluginSwitch
        /// </summary>
        public int WebWxPluginSwitch { get; set; }
        /// <summary>
        /// 1有头像
        /// </summary>
        public int HeadImgFlag { get; set; }
        /// <summary>
        /// 含义未知
        /// 公众号是0，其他包括1，16，17，49，129，145，177
        /// </summary>
        public int SnsFlag { get; set; }
    }

}
