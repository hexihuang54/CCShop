using CCShop.Common.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CCShop.Common.Util
{
    public class HttpUtil
    {

        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";


        #region  获取客户端IP地址统一方法

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns>若失败则返回回送地址</returns>
        public static string GetIP()
        {
            string userHostAddress = string.Empty;

            try
            {
                if (HttpContext.Current.Request.Headers["HTTP_VIA"] != "")
                {
                    //如果客户端使用了代理服务器，则利用HTTP_X_FORWARDED_FOR找到客户端IP地址
                    if (HttpContext.Current.Request.Headers["HTTP_X_FORWARDED_FOR"] != "")
                    {
                        userHostAddress = HttpContext.Current.Request.Headers["HTTP_X_FORWARDED_FOR"].ToString().Split(',')[0].Trim();
                    }
                }

                //否则直接读取REMOTE_ADDR获取客户端IP地址
                if (string.IsNullOrEmpty(userHostAddress))
                {
                    userHostAddress = HttpContext.Current.Request.Headers["REMOTE_ADDR"];
                }

                //前两者均失败，则利用Request.UserHostAddress属性获取IP地址，但此时无法确定该IP是客户端IP还是代理IP
                if (string.IsNullOrEmpty(userHostAddress))
                {
                    userHostAddress = HttpContext.Current.Connection.RemoteIpAddress.ToString();
                }

                //最后判断获取是否成功，并检查IP地址的格式（检查其格式非常重要）
                if (!string.IsNullOrEmpty(userHostAddress) && IsIP(userHostAddress))
                {
                    return userHostAddress;
                }

                if (userHostAddress.Contains("::1"))
                {
                    return "";
                }

                return userHostAddress;
            }
            catch (Exception ex)
            {
                return userHostAddress;
            }

        }

        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        #endregion

        public static string Get(string url, string data, IDictionary<string, string> parameters = null, Encoding requestEncoding = null)
        {
            string result = "";

            if (!url.Contains("http://") && !url.Contains("https://"))
                url = "http://" + url;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 100000;

            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(data);
                    request.ContentLength = buffer.Length;
                    request.GetRequestStream().Write(buffer, 0, buffer.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = decompressGZipIfNeed(response, Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                //throw e;
            }

            return result;
        }

        public static string Post(string url, IDictionary<string, string> parameters = null, Encoding requestEncoding = null)
        {
            string result = "";

            if (!url.Contains("http://") && !url.Contains("https://"))
                url = "http://" + url;

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = DefaultUserAgent;

            try
            {
                //如果需要POST数据  
                if (!(parameters == null || parameters.Count == 0))
                {
                    StringBuilder buffer = new StringBuilder();
                    int i = 0;
                    foreach (string key in parameters.Keys)
                    {
                        if (i > 0)
                        {
                            buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                        }
                        else
                        {
                            buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        }
                        i++;
                    }
                    byte[] bytedata = requestEncoding.GetBytes(buffer.ToString());
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(bytedata, 0, bytedata.Length);
                    }
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = decompressGZipIfNeed(response, Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                //throw e;
            }

            return result;
        }

        private static StreamReader decompressGZipIfNeed(HttpWebResponse response, Encoding encoding)
        {
            if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
            {
                return new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress), encoding);
            }
            else
            {
                return new StreamReader(response.GetResponseStream(), encoding);
            }
        }

        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //如果需要POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                byte[] data = requestEncoding.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }

        /// <summary>
        /// 向服务器提交数据
        /// </summary>
        /// <param name="url">远程访问的地址</param>
        /// <param name="parameters">参数名,参数值</param>
        /// <param name="method">Http页面请求方法</param>
        /// <returns>远程页面调用结果</returns>
        public static string PostDataToServer(string url, IDictionary<string, string> parameters, string method)
        {
            try
            {
                Encoding encoding = Encoding.GetEncoding("utf-8");
                HttpWebResponse response;
                if (method.ToUpper() == "POST")
                {
                    response = CreatePostHttpResponse(url, parameters, null, null, encoding, null);
                }
                else
                {
                    response = CreateGetHttpResponse(url, null, null, null);
                }
                Stream streamIn = response.GetResponseStream();
                StreamReader reader = new StreamReader(streamIn, encoding);
                return reader.ReadToEnd();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            return request.GetResponse() as HttpWebResponse;
        }

        public static HttpClient GetClient()
        {
            var client = new HttpClient();
            client.GetAsync("");
            HttpRequestMessage ms = new HttpRequestMessage();
            ms.Content = null;
            ms.Method = HttpMethod.Post;
            return client;
        }

    }
}
