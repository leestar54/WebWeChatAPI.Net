using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Leestar54.WeChat.WebAPI
{
    public class FormDataItem
    {
        public string Name;
        public string Value;
        public bool isFile;
        public string FileName;
        public byte[] Content;
        public int ContentLength;

        public FormDataItem(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public FormDataItem(string name, string filename, byte[] content, int length)
        {
            isFile = true;
            Name = name;
            FileName = filename;
            Content = content;
            ContentLength = length;
        }
    }

    public class HttpClient
    {
        public HttpClient()
        {
            ServicePointManager.DefaultConnectionLimit = 512;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private CookieContainer cookieContainer = new CookieContainer();

        public CookieContainer CookieContainer
        {
            get
            {
                return cookieContainer;
            }
        }

        public string Referer = string.Empty;

        public string GetString(string url)
        {
            string result = string.Empty;
            Stream stream = Retry<Stream>(() => { return GetResponseStream("GET", url); });
            using (stream)
            {
                StreamReader sr = new StreamReader(stream);
                result = sr.ReadToEnd();
            }
            return result;
        }

        public string GetStringOnce(string url)
        {
            string result = string.Empty;
            Stream stream = GetResponseStream("GET", url);
            using (stream)
            {
                StreamReader sr = new StreamReader(stream);
                result = sr.ReadToEnd();
            }
            return result;
        }

        public string PostMutipart(string url, List<FormDataItem> dataList)
        {
            string result = string.Empty;
            var boundary = "----WebKitFormBoundary" + Guid.NewGuid().ToString("N").Substring(0, 16);
            Stream stream = Retry<Stream>(() =>
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "POST";
                request.Accept = "*/*";
                request = PretendWechat(request);
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                var sw = new StreamWriter(request.GetRequestStream());
                foreach (var item in dataList)
                {
                    if (item.isFile)
                    {
                        sw.Write(string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; {2}=\"{3}\"\r\nContent-Type: {4}\r\n\r\n", boundary, item.Name, item.Name, item.FileName, MimeMapping.GetMimeMapping(item.FileName)));
                        sw.Flush();
                        sw.BaseStream.Write(item.Content, 0, item.ContentLength);
                        sw.Write("\r\n");
                    }
                    else
                    {
                        sw.Write(string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", boundary, item.Name, item.Value));
                    }
                    sw.Flush();
                }
                sw.Write("--" + boundary + "--\r\n");
                sw.Flush();
                sw.Close();
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                return response.GetResponseStream();
            });
            using (stream)
            {
                StreamReader sr = new StreamReader(stream);
                result = sr.ReadToEnd();
            }
            return result;
        }

        public string PostFormString(string url, string body)
        {
            string result = string.Empty;
            Stream stream = Retry<Stream>(() =>
            {
                return GetResponseStream("POST", url, "application/x-www-form-urlencoded", body);
            });
            using (stream)
            {
                StreamReader sr = new StreamReader(stream);
                result = sr.ReadToEnd();
            }
            return result;
        }

        public T PostJson<T>(string url, object body)
        {
            string result = string.Empty;
            Stream stream = Retry<Stream>(() =>
            {
                return GetResponseStream("POST", url, "application/json;charset=UTF-8", JsonConvert.SerializeObject(body, Newtonsoft.Json.Formatting.None));
            });
            using (stream)
            {
                StreamReader sr = new StreamReader(stream);
                result = sr.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<T>(result);
        }


        public Stream GetResponseStream(string method, string url, string contentType = null, object body = null)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = method;
            request = PretendWechat(request);
            if (contentType != null)
            {
                request.ContentType = contentType;
            }
            if (body != null)
            {
                StreamWriter sw = new StreamWriter(request.GetRequestStream());
                sw.Write(body);
                sw.Flush();
                sw.Close();
            }
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            return stream;
        }

        public Image GetImage(string url)
        {
            Image result;
            Stream stream = Retry<Stream>(() => { return GetResponseStream("GET", url); });
            using (stream)
            {
                result = Image.FromStream(stream);
            }
            return result;
        }

        public HttpWebRequest PretendWechat(HttpWebRequest request)
        {
#if !DEBUG
            //request.Proxy = null;
#endif
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.Headers.Add("Accept-Encoding", "gzip,deflate,br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,en;q=0.6,zh-TW;q=0.4");
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = false;
            request.ServicePoint.Expect100Continue = false;
            request.Referer = Referer;
            request.Timeout = 35000;
            if (Referer != string.Empty)
            {
                request.Headers.Add("Origin", Referer);
            }
            return request;
        }


        /// <summary>
        /// 三次重试机制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        private T Retry<T>(Func<T> func)
        {
            int err = 0;
            while (err < 3)
            {
                try
                {
                    return func();
                }
                catch (WebException webExp)
                {
                    err++;
                    Thread.Sleep(5000);
                    if (err > 2)
                    {
                        throw webExp;
                    }
                }
            }
            return func();
        }
    }

    public static class MimeMapping
    {
        private static Dictionary<string, string> _mimeMappingTable;

        private static void AddMimeMapping(string extension, string MimeType)
        {
            MimeMapping._mimeMappingTable.Add(extension, MimeType);
        }

        public static string GetMimeMapping(string FileName)
        {
            string text = null;
            int num = FileName.LastIndexOf('.');
            if (0 < num && num > FileName.LastIndexOf('\\'))
            {
                text = (string)MimeMapping._mimeMappingTable[FileName.Substring(num)];
            }
            if (text == null)
            {
                text = (string)MimeMapping._mimeMappingTable[".*"];
            }
            return text;
        }

        static MimeMapping()
        {
            MimeMapping._mimeMappingTable = new Dictionary<string, string>(190, StringComparer.CurrentCultureIgnoreCase);
            MimeMapping.AddMimeMapping(".323", "text/h323");
            MimeMapping.AddMimeMapping(".asx", "video/x-ms-asf");
            MimeMapping.AddMimeMapping(".acx", "application/internet-property-stream");
            MimeMapping.AddMimeMapping(".ai", "application/postscript");
            MimeMapping.AddMimeMapping(".aif", "audio/x-aiff");
            MimeMapping.AddMimeMapping(".aiff", "audio/aiff");
            MimeMapping.AddMimeMapping(".axs", "application/olescript");
            MimeMapping.AddMimeMapping(".aifc", "audio/aiff");
            MimeMapping.AddMimeMapping(".asr", "video/x-ms-asf");
            MimeMapping.AddMimeMapping(".avi", "video/x-msvideo");
            MimeMapping.AddMimeMapping(".asf", "video/x-ms-asf");
            MimeMapping.AddMimeMapping(".au", "audio/basic");
            MimeMapping.AddMimeMapping(".application", "application/x-ms-application");
            MimeMapping.AddMimeMapping(".bin", "application/octet-stream");
            MimeMapping.AddMimeMapping(".bas", "text/plain");
            MimeMapping.AddMimeMapping(".bcpio", "application/x-bcpio");
            MimeMapping.AddMimeMapping(".bmp", "image/bmp");
            MimeMapping.AddMimeMapping(".cdf", "application/x-cdf");
            MimeMapping.AddMimeMapping(".cat", "application/vndms-pkiseccat");
            MimeMapping.AddMimeMapping(".crt", "application/x-x509-ca-cert");
            MimeMapping.AddMimeMapping(".c", "text/plain");
            MimeMapping.AddMimeMapping(".css", "text/css");
            MimeMapping.AddMimeMapping(".cer", "application/x-x509-ca-cert");
            MimeMapping.AddMimeMapping(".crl", "application/pkix-crl");
            MimeMapping.AddMimeMapping(".cmx", "image/x-cmx");
            MimeMapping.AddMimeMapping(".csh", "application/x-csh");
            MimeMapping.AddMimeMapping(".cod", "image/cis-cod");
            MimeMapping.AddMimeMapping(".cpio", "application/x-cpio");
            MimeMapping.AddMimeMapping(".clp", "application/x-msclip");
            MimeMapping.AddMimeMapping(".crd", "application/x-mscardfile");
            MimeMapping.AddMimeMapping(".deploy", "application/octet-stream");
            MimeMapping.AddMimeMapping(".dll", "application/x-msdownload");
            MimeMapping.AddMimeMapping(".dot", "application/msword");
            MimeMapping.AddMimeMapping(".doc", "application/msword");
            MimeMapping.AddMimeMapping(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            MimeMapping.AddMimeMapping(".dvi", "application/x-dvi");
            MimeMapping.AddMimeMapping(".dir", "application/x-director");
            MimeMapping.AddMimeMapping(".dxr", "application/x-director");
            MimeMapping.AddMimeMapping(".der", "application/x-x509-ca-cert");
            MimeMapping.AddMimeMapping(".dib", "image/bmp");
            MimeMapping.AddMimeMapping(".dcr", "application/x-director");
            MimeMapping.AddMimeMapping(".disco", "text/xml");
            MimeMapping.AddMimeMapping(".exe", "application/octet-stream");
            MimeMapping.AddMimeMapping(".etx", "text/x-setext");
            MimeMapping.AddMimeMapping(".evy", "application/envoy");
            MimeMapping.AddMimeMapping(".eml", "message/rfc822");
            MimeMapping.AddMimeMapping(".eps", "application/postscript");
            MimeMapping.AddMimeMapping(".flr", "x-world/x-vrml");
            MimeMapping.AddMimeMapping(".fif", "application/fractals");
            MimeMapping.AddMimeMapping(".gtar", "application/x-gtar");
            MimeMapping.AddMimeMapping(".gif", "image/gif");
            MimeMapping.AddMimeMapping(".gz", "application/x-gzip");
            MimeMapping.AddMimeMapping(".hta", "application/hta");
            MimeMapping.AddMimeMapping(".htc", "text/x-component");
            MimeMapping.AddMimeMapping(".htt", "text/webviewhtml");
            MimeMapping.AddMimeMapping(".h", "text/plain");
            MimeMapping.AddMimeMapping(".hdf", "application/x-hdf");
            MimeMapping.AddMimeMapping(".hlp", "application/winhlp");
            MimeMapping.AddMimeMapping(".html", "text/html");
            MimeMapping.AddMimeMapping(".htm", "text/html");
            MimeMapping.AddMimeMapping(".hqx", "application/mac-binhex40");
            MimeMapping.AddMimeMapping(".isp", "application/x-internet-signup");
            MimeMapping.AddMimeMapping(".iii", "application/x-iphone");
            MimeMapping.AddMimeMapping(".ief", "image/ief");
            MimeMapping.AddMimeMapping(".ivf", "video/x-ivf");
            MimeMapping.AddMimeMapping(".ins", "application/x-internet-signup");
            MimeMapping.AddMimeMapping(".ico", "image/x-icon");
            MimeMapping.AddMimeMapping(".jpg", "image/jpeg");
            MimeMapping.AddMimeMapping(".jfif", "image/pjpeg");
            MimeMapping.AddMimeMapping(".jpe", "image/jpeg");
            MimeMapping.AddMimeMapping(".jpeg", "image/jpeg");
            MimeMapping.AddMimeMapping(".js", "application/x-javascript");
            MimeMapping.AddMimeMapping(".lsx", "video/x-la-asf");
            MimeMapping.AddMimeMapping(".latex", "application/x-latex");
            MimeMapping.AddMimeMapping(".lsf", "video/x-la-asf");
            MimeMapping.AddMimeMapping(".manifest", "application/x-ms-manifest");
            MimeMapping.AddMimeMapping(".mhtml", "message/rfc822");
            MimeMapping.AddMimeMapping(".mny", "application/x-msmoney");
            MimeMapping.AddMimeMapping(".mht", "message/rfc822");
            MimeMapping.AddMimeMapping(".mid", "audio/mid");
            MimeMapping.AddMimeMapping(".mpv2", "video/mpeg");
            MimeMapping.AddMimeMapping(".man", "application/x-troff-man");
            MimeMapping.AddMimeMapping(".mvb", "application/x-msmediaview");
            MimeMapping.AddMimeMapping(".mpeg", "video/mpeg");
            MimeMapping.AddMimeMapping(".m3u", "audio/x-mpegurl");
            MimeMapping.AddMimeMapping(".mdb", "application/x-msaccess");
            MimeMapping.AddMimeMapping(".mpp", "application/vnd.ms-project");
            MimeMapping.AddMimeMapping(".m1v", "video/mpeg");
            MimeMapping.AddMimeMapping(".mpa", "video/mpeg");
            MimeMapping.AddMimeMapping(".me", "application/x-troff-me");
            MimeMapping.AddMimeMapping(".m13", "application/x-msmediaview");
            MimeMapping.AddMimeMapping(".movie", "video/x-sgi-movie");
            MimeMapping.AddMimeMapping(".m14", "application/x-msmediaview");
            MimeMapping.AddMimeMapping(".mpe", "video/mpeg");
            MimeMapping.AddMimeMapping(".mp2", "video/mpeg");
            MimeMapping.AddMimeMapping(".mov", "video/quicktime");
            MimeMapping.AddMimeMapping(".mp3", "audio/mpeg");
            MimeMapping.AddMimeMapping(".mpg", "video/mpeg");
            MimeMapping.AddMimeMapping(".ms", "application/x-troff-ms");
            MimeMapping.AddMimeMapping(".nc", "application/x-netcdf");
            MimeMapping.AddMimeMapping(".nws", "message/rfc822");
            MimeMapping.AddMimeMapping(".oda", "application/oda");
            MimeMapping.AddMimeMapping(".ods", "application/oleobject");
            MimeMapping.AddMimeMapping(".pmc", "application/x-perfmon");
            MimeMapping.AddMimeMapping(".p7r", "application/x-pkcs7-certreqresp");
            MimeMapping.AddMimeMapping(".p7b", "application/x-pkcs7-certificates");
            MimeMapping.AddMimeMapping(".p7s", "application/pkcs7-signature");
            MimeMapping.AddMimeMapping(".pmw", "application/x-perfmon");
            MimeMapping.AddMimeMapping(".ps", "application/postscript");
            MimeMapping.AddMimeMapping(".p7c", "application/pkcs7-mime");
            MimeMapping.AddMimeMapping(".pbm", "image/x-portable-bitmap");
            MimeMapping.AddMimeMapping(".ppm", "image/x-portable-pixmap");
            MimeMapping.AddMimeMapping(".pub", "application/x-mspublisher");
            MimeMapping.AddMimeMapping(".pnm", "image/x-portable-anymap");
            MimeMapping.AddMimeMapping(".png", "image/png");
            MimeMapping.AddMimeMapping(".pml", "application/x-perfmon");
            MimeMapping.AddMimeMapping(".p10", "application/pkcs10");
            MimeMapping.AddMimeMapping(".pfx", "application/x-pkcs12");
            MimeMapping.AddMimeMapping(".p12", "application/x-pkcs12");
            MimeMapping.AddMimeMapping(".pdf", "application/pdf");
            MimeMapping.AddMimeMapping(".pps", "application/vnd.ms-powerpoint");
            MimeMapping.AddMimeMapping(".p7m", "application/pkcs7-mime");
            MimeMapping.AddMimeMapping(".pko", "application/vndms-pkipko");
            MimeMapping.AddMimeMapping(".ppt", "application/vnd.ms-powerpoint");
            MimeMapping.AddMimeMapping(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
            MimeMapping.AddMimeMapping(".pmr", "application/x-perfmon");
            MimeMapping.AddMimeMapping(".pma", "application/x-perfmon");
            MimeMapping.AddMimeMapping(".pot", "application/vnd.ms-powerpoint");
            MimeMapping.AddMimeMapping(".prf", "application/pics-rules");
            MimeMapping.AddMimeMapping(".pgm", "image/x-portable-graymap");
            MimeMapping.AddMimeMapping(".qt", "video/quicktime");
            MimeMapping.AddMimeMapping(".ra", "audio/x-pn-realaudio");
            MimeMapping.AddMimeMapping(".rgb", "image/x-rgb");
            MimeMapping.AddMimeMapping(".ram", "audio/x-pn-realaudio");
            MimeMapping.AddMimeMapping(".rmi", "audio/mid");
            MimeMapping.AddMimeMapping(".ras", "image/x-cmu-raster");
            MimeMapping.AddMimeMapping(".roff", "application/x-troff");
            MimeMapping.AddMimeMapping(".rtf", "application/rtf");
            MimeMapping.AddMimeMapping(".rtx", "text/richtext");
            MimeMapping.AddMimeMapping(".sv4crc", "application/x-sv4crc");
            MimeMapping.AddMimeMapping(".spc", "application/x-pkcs7-certificates");
            MimeMapping.AddMimeMapping(".setreg", "application/set-registration-initiation");
            MimeMapping.AddMimeMapping(".snd", "audio/basic");
            MimeMapping.AddMimeMapping(".stl", "application/vndms-pkistl");
            MimeMapping.AddMimeMapping(".setpay", "application/set-payment-initiation");
            MimeMapping.AddMimeMapping(".stm", "text/html");
            MimeMapping.AddMimeMapping(".shar", "application/x-shar");
            MimeMapping.AddMimeMapping(".sh", "application/x-sh");
            MimeMapping.AddMimeMapping(".sit", "application/x-stuffit");
            MimeMapping.AddMimeMapping(".spl", "application/futuresplash");
            MimeMapping.AddMimeMapping(".sct", "text/scriptlet");
            MimeMapping.AddMimeMapping(".scd", "application/x-msschedule");
            MimeMapping.AddMimeMapping(".sst", "application/vndms-pkicertstore");
            MimeMapping.AddMimeMapping(".src", "application/x-wais-source");
            MimeMapping.AddMimeMapping(".sv4cpio", "application/x-sv4cpio");
            MimeMapping.AddMimeMapping(".tex", "application/x-tex");
            MimeMapping.AddMimeMapping(".tgz", "application/x-compressed");
            MimeMapping.AddMimeMapping(".t", "application/x-troff");
            MimeMapping.AddMimeMapping(".tar", "application/x-tar");
            MimeMapping.AddMimeMapping(".tr", "application/x-troff");
            MimeMapping.AddMimeMapping(".tif", "image/tiff");
            MimeMapping.AddMimeMapping(".txt", "text/plain");
            MimeMapping.AddMimeMapping(".texinfo", "application/x-texinfo");
            MimeMapping.AddMimeMapping(".trm", "application/x-msterminal");
            MimeMapping.AddMimeMapping(".tiff", "image/tiff");
            MimeMapping.AddMimeMapping(".tcl", "application/x-tcl");
            MimeMapping.AddMimeMapping(".texi", "application/x-texinfo");
            MimeMapping.AddMimeMapping(".tsv", "text/tab-separated-values");
            MimeMapping.AddMimeMapping(".ustar", "application/x-ustar");
            MimeMapping.AddMimeMapping(".uls", "text/iuls");
            MimeMapping.AddMimeMapping(".vcf", "text/x-vcard");
            MimeMapping.AddMimeMapping(".wps", "application/vnd.ms-works");
            MimeMapping.AddMimeMapping(".wav", "audio/wav");
            MimeMapping.AddMimeMapping(".wrz", "x-world/x-vrml");
            MimeMapping.AddMimeMapping(".wri", "application/x-mswrite");
            MimeMapping.AddMimeMapping(".wks", "application/vnd.ms-works");
            MimeMapping.AddMimeMapping(".wmf", "application/x-msmetafile");
            MimeMapping.AddMimeMapping(".wcm", "application/vnd.ms-works");
            MimeMapping.AddMimeMapping(".wrl", "x-world/x-vrml");
            MimeMapping.AddMimeMapping(".wdb", "application/vnd.ms-works");
            MimeMapping.AddMimeMapping(".wsdl", "text/xml");
            MimeMapping.AddMimeMapping(".xap", "application/x-silverlight-app");
            MimeMapping.AddMimeMapping(".xml", "text/xml");
            MimeMapping.AddMimeMapping(".xlm", "application/vnd.ms-excel");
            MimeMapping.AddMimeMapping(".xaf", "x-world/x-vrml");
            MimeMapping.AddMimeMapping(".xla", "application/vnd.ms-excel");
            MimeMapping.AddMimeMapping(".xls", "application/vnd.ms-excel");
            MimeMapping.AddMimeMapping(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            MimeMapping.AddMimeMapping(".xof", "x-world/x-vrml");
            MimeMapping.AddMimeMapping(".xlt", "application/vnd.ms-excel");
            MimeMapping.AddMimeMapping(".xlc", "application/vnd.ms-excel");
            MimeMapping.AddMimeMapping(".xsl", "text/xml");
            MimeMapping.AddMimeMapping(".xbm", "image/x-xbitmap");
            MimeMapping.AddMimeMapping(".xlw", "application/vnd.ms-excel");
            MimeMapping.AddMimeMapping(".xpm", "image/x-xpixmap");
            MimeMapping.AddMimeMapping(".xwd", "image/x-xwindowdump");
            MimeMapping.AddMimeMapping(".xsd", "text/xml");
            MimeMapping.AddMimeMapping(".z", "application/x-compress");
            MimeMapping.AddMimeMapping(".zip", "application/x-zip-compressed");
            MimeMapping.AddMimeMapping(".*", "application/octet-stream");
        }
    }
}