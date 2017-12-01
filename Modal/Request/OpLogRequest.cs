using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal.Request
{
    public enum CmdIdType
    {
        TOPCONTACT = 3,
        MODREMARKNAME = 2
    }
    class OpLogRequest
    {
        /// <summary>
        /// @506400cfe028ac7d23f80a4054e5f930
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// TOPCONTACT: 3,
        /// MODREMARKNAME: 2
        /// </summary>
        public CmdIdType CmdId { get; set; }
        /// <summary>
        /// Leestar
        /// </summary>
        public string RemarkName { get; set; }
        /// <summary>
        /// BaseRequest
        /// </summary>
        public BaseRequest BaseRequest { get; set; }
    }

    class TopOpLogRequest : OpLogRequest
    {
        public int OP { get; set; }
    }

}
