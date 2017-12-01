using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal
{
    public class MPArticle
    {
        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 简述
        /// </summary>
        public string Digest { get; set; }
        /// <summary>
        /// 文章图片地址，可以直接读取
        /// </summary>
        public string Cover { get; set; }
        /// <summary>
        /// 文章地址
        /// </summary>
        public string Url { get; set; }
    }
}
