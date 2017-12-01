using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal.Request
{
    public class UploadMediaRequest
    {
        /// <summary>
        /// UPLOAD_MEDIA_TYPE_IMAGE: 1,
        /// UPLOAD_MEDIA_TYPE_VIDEO: 2,
        /// UPLOAD_MEDIA_TYPE_AUDIO: 3,
        /// UPLOAD_MEDIA_TYPE_ATTACHMENT: 4,
        /// </summary>
        public int UploadType { get; set; }
        /// <summary>
        /// BaseRequest
        /// </summary>
        public BaseRequest BaseRequest { get; set; }
        /// <summary>
        /// ClientMediaId
        /// </summary>
        public Int64 ClientMediaId { get; set; }
        /// <summary>
        /// TotalLen
        /// </summary>
        public long TotalLen { get; set; }
        /// <summary>
        /// StartPos
        /// </summary>
        public int StartPos { get; set; }
        /// <summary>
        /// DataLen
        /// </summary>
        public long DataLen { get; set; }
        /// <summary>
        /// MediaType
        /// </summary>
        public int MediaType { get; set; }
        /// <summary>
        /// @1a4f170f4ab3053a42cfa7e1f5ec9b74e556a38fef594d37d87a8c2dee52a6d3
        /// </summary>
        public string FromUserName { get; set; }
        /// <summary>
        /// @8dfe20714560594a51d8beb0ec8ecf26
        /// </summary>
        public string ToUserName { get; set; }
        /// <summary>
        /// 897febd45c211bf7c3ab15cda559a156
        /// </summary>
        public string FileMd5 { get; set; }
    }
}
