using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal.Request
{
    public class SyncRequest
    {
        /// <summary>
        /// BaseRequest
        /// </summary>
        public BaseRequest BaseRequest { get; set; }
        /// <summary>
        /// SyncKey
        /// </summary>
        public SyncKey SyncKey { get; set; }
        /// <summary>
        /// Rr
        /// </summary>
        public int rr { get; set; }
    }
}
