using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal
{
    public class KeyValPair
    {
        /// <summary>
        /// Key
        /// </summary>
        public int Key { get; set; }
        /// <summary>
        /// Val
        /// </summary>
        public int Val { get; set; }
    }

    /// <summary>
    /// synccheck轮询检测用
    /// </summary>
    public class SyncKey
    {
        /// <summary>
        /// Count
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// List
        /// </summary>
        public List<KeyValPair> List { get; set; }

        private Int64 step = 0;
        /// <summary>
        /// 每次请求都要自增1
        /// </summary>
        public Int64 Step
        {
            get
            {
                if (step == 0)
                {
                    step = Convert.ToInt64(OtherUtils.GetJavaTimeStamp());
                }
                else
                {
                    step++;
                }
                return step;
            }
        }

        public override string ToString()
        {
            string keyparam = string.Empty;
            for (int i = 0; i < List.Count; i++)
            {
                keyparam += List[i].Key.ToString() + "_" + List[i].Val.ToString() + "%7C";
            }
            return keyparam;
        }
    }

}
