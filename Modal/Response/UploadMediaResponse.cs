using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal.Response
{
    public class UploadMediaResponse
    {
        /// <summary>
        /// BaseResponse
        /// </summary>
        public BaseResponse BaseResponse { get; set; }
        /// <summary>
        /// @crypt_fba0e9f6_5674d032da480242db64ddce7ec63c7184242805ddab26f246cb6fd8be030f1a60fbe141921f6b631ecec59b6ef274b4fb91a618e378bd43d9dac158d3cf63101e1da5be51dac4a1a2490f819c07a428ee5295e6639d9c65ab472ada21ecac9adb26e5f3151f3988a61a7a9a36cb8266ca3e9a310f80265d2a2608d9d24716f959a0a73f0242c0a173a24b781f438ec2b414474c6e17ecdb8800e98540fd6bcd3fa1758ffa515d89b200f20b3cb6b24e3caabfd1e26420c2702a3e63517636da5e0f819eddae8bb2a608b07b1e21fa5dc0acb644f95616be16cb7033b52d44e91f631a3b61f2dffd766074d9cc2b17921b218d23f9c427c202a8b2de8d22e97698aae88846bb1f0869dee87028276e04be7509c3f3f220e9971cb7975a6fa0efd26a20a7ae9f4ba9372b3e79a181c912f8f43d0002cf924ab1858769a62654dd
        /// </summary>
        public string MediaId { get; set; }
        /// <summary>
        /// 图片大小
        /// </summary>
        public int StartPos { get; set; }
        /// <summary>
        /// 缩略图高度，媒体文件会有，用来设置缩略图控件图片大小
        /// </summary>
        public int CDNThumbImgHeight { get; set; }
        /// <summary>
        /// 缩略图宽度，媒体文件会有，用来设置缩略图控件图片大小
        /// </summary>
        public int CDNThumbImgWidth { get; set; }
    }
}
