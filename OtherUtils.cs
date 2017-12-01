using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI
{
    public class OtherUtils
    {
        public static void Debug(string info, string tag = "debug")
        {
            System.Diagnostics.Debug.WriteLine(tag + " " + info);
        }

        /// <summary>
        /// debug输出错误，并且返回格式化的错误
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static string Debug(Exception err)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("****************************异常文本****************************");
            sb.AppendLine("【出现时间】：" + DateTime.Now.ToString());
            if (err != null)
            {
                sb.AppendLine("【异常类型】：" + err.GetType().Name);
                sb.AppendLine("【异常信息】：" + err.Message);
                sb.AppendLine("【堆栈调用】：" + err.StackTrace);
            }
            sb.AppendLine("***************************************************************");
            System.Diagnostics.Debug.WriteLine(sb.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// 获取时间戳，10位数字
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 模拟获取微信接口中的r
        /// </summary>
        /// <returns></returns>
        public static int Get_r()
        {
            string timespan = OtherUtils.GetTimeStamp();
            return (int)(Convert.ToInt64(timespan) / 888);
        }

        /// <summary>  
        /// 获取时间戳，13位数字，兼容java
        /// </summary>  
        /// <returns></returns>  
        public static Int64 GetJavaTimeStamp()
        {
            return Convert.ToInt64(GetTimeStamp() + GetRandomNumber(3));
        }

        /// <summary>
        /// 时间戳转datetime
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime StampToDateTime(string timeStamp)
        {
            DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dateTimeStart.Add(toNow);
        }

        /// <summary>
        /// 生成随机数，保证第一位不是0
        /// </summary>
        /// <param name="num">长度</param>
        /// <returns></returns>
        public static string GetRandomNumber(int length)
        {
            char[] Pattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            //, 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string randcode = "";
            int n = Pattern.Length;
            Random random = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < length; i++)
            {
                int rnd;
                if (i != 0)
                    rnd = random.Next(0, n);
                else
                    rnd = random.Next(1, n);
                randcode += Pattern[rnd];
            }
            return randcode;
        }

        public static string GetFileMD5Hash(string fileName)
        {
            return GetFileMD5Hash(new FileInfo(fileName));
        }
        public static string GetFileMD5Hash(FileInfo fileInfo)
        {
            try
            {
                FileStream file = fileInfo.OpenRead();
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5Hash() fail,error:" + ex.Message);
            }
        }
    }
}
