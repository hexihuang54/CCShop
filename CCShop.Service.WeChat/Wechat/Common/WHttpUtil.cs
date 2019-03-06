using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Cache;
using CCShop.Log4Net;

namespace CCShop.Service.WeChat.Common
{
    public class WHttpUtil
    {

        public static readonly string JsonType = "application/json";

        #region HttpWebRequest模式请求方法

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }

        public static string Post(string xml, string url, bool isUseCert, int timeout)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接

            string result = "";//返回结果

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;

            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 512;

                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.Timeout = timeout * 1000;
                request.KeepAlive = false;
                request.AllowAutoRedirect = true;
                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                request.MaximumResponseHeadersLength = 1024 * 100;

                //设置代理服务器
                //WebProxy proxy = new WebProxy();                          //定义一个网关对象
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);           //网关服务器端口:端口
                //request.Proxy = proxy;

                //设置POST的数据类型和长度
                request.ContentType = "text/xml";
                byte[] data = Encoding.UTF8.GetBytes(xml);
                request.ContentLength = data.Length;

                //是否使用证书
                if (isUseCert)
                {
                    string path = ""; //HttpContext.Current.Request.PhysicalApplicationPath;
                    string SSLCERT_PATH = "";
                    string SSLCERT_PASSWORD = "";
                    X509Certificate2 cert = new X509Certificate2(path + SSLCERT_PATH, SSLCERT_PASSWORD);
                    request.ClientCertificates.Add(cert);
                }

                //往服务器写入数据
                reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();

                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();

                //获取服务端返回数据
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    result = sr.ReadToEnd();
                }
            }
            catch (System.Threading.ThreadAbortException e)
            {
                Log.Err(e, "HttpService, Thread - caught ThreadAbortException - resetting");
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                Log.Err(e, "HttpService");
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    Log.Err(e, "HttpService,StatusCode : " + ((HttpWebResponse)e.Response).StatusCode);
                    Log.Err(e, "HttpService,StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription);
                }
                throw new Exception(e.ToString());
            }
            catch (Exception e)
            {
                Log.Err(e, "HttpService");
                throw new Exception(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        public static string Get(string url, string data)
        {
            string result = "";

            try
            {
                if (!url.Contains("http://") && !url.Contains("https://"))
                    url = "http://" + url;

                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 512;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 300000;
                request.MaximumResponseHeadersLength = 1024 * 100;

                if (!string.IsNullOrEmpty(data))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(data);
                    request.ContentLength = buffer.Length;
                    request.GetRequestStream().Write(buffer, 0, buffer.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = DecompressGZipIfNeed(response, Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        /// <summary>
        /// 处理http GET请求，返回数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
        public static string Get(string url)
        {
            System.GC.Collect();
            string result = "";

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            //请求url以获取数据
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 512;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";
                request.KeepAlive = false;
                request.Timeout = 6 * 6 * 10 * 1000;
                request.MaximumResponseHeadersLength = 1024 * 100;
                request.AllowAutoRedirect = true;
                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

                //设置代理
                //WebProxy proxy = new WebProxy();
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);
                //request.Proxy = proxy;

                //获取服务器返回
                response = (HttpWebResponse)request.GetResponse();

                //获取HTTP返回数据
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    result = sr.ReadToEnd();
                }
            }
            catch (System.Threading.ThreadAbortException e)
            {
                Log.Err(e, "HttpService, Thread - caught ThreadAbortException - resetting");
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                Log.Err(e, "HttpService");
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    Log.Err(e, "HttpService,StatusCode : " + ((HttpWebResponse)e.Response).StatusCode);
                    Log.Err(e, "HttpService,StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription);
                }
                throw new Exception(e.ToString());
            }
            catch (Exception e)
            {
                Log.Err(e, "HttpService");
                throw new Exception(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }

                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        private static StreamReader DecompressGZipIfNeed(HttpWebResponse response, Encoding encoding)
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

        public static string HttpMediaIdPost(string url, byte[] bf, string filename)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            CookieContainer container = new CookieContainer();
            request.CookieContainer = container;
            request.AllowAutoRedirect = true;
            request.Method = "POST";
            string boundary = DateTime.Now.Ticks.ToString("X");//随机分隔线
            request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
            byte[] startBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            string strHeader = string.Format("Content-Disposition:form-data;name=\"media\";filename=\"{0}\"\r\n Content-Type:application/octet-stream\r\n\r\n", filename);
            byte[] headerByte = Encoding.UTF8.GetBytes(strHeader);

            Stream postStream = request.GetRequestStream();
            postStream.Write(startBytes, 0, startBytes.Length);
            postStream.Write(headerByte, 0, headerByte.Length);
            postStream.Write(bf, 0, bf.Length);
            postStream.Write(endBytes, 0, endBytes.Length);
            postStream.Close();

            //发送请求
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
            string result = reader.ReadToEnd();
            reader.Close();
            responseStream.Close();
            return result;
        }

        #endregion


        #region HttpClient模式请求方法

        /// <summary>
        /// 同步GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="timeout">请求响应超时时间，单位/s(默认100秒)</param>
        /// <returns></returns>
        public static string HttpGet(string url, Dictionary<string, string> headers = null, int timeout = 0)
        {
            using (HttpClient client = new HttpClient())
            {
                client.MaxResponseContentBufferSize = 1024 * 1024 * 10;

                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                if (timeout > 0)
                {
                    client.Timeout = new TimeSpan(0, 0, timeout);
                }

                Byte[] resultBytes = client.GetByteArrayAsync(url).Result;
                return Encoding.UTF8.GetString(resultBytes);
            }
        }

        /// <summary>
        /// 异步GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="timeout">请求响应超时时间，单位/s(默认100秒)</param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string url, Dictionary<string, string> headers = null, int timeout = 0)
        {
            using (HttpClient client = new HttpClient())
            {
                client.MaxResponseContentBufferSize = 1024 * 1024 * 10;

                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                if (timeout > 0)
                {
                    client.Timeout = new TimeSpan(0, 0, timeout);
                }
                Byte[] resultBytes = await client.GetByteArrayAsync(url);
                return Encoding.Default.GetString(resultBytes);
            }
        }

        /// <summary>
        /// 同步POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <param name="timeout">请求响应超时时间，单位/s(默认100秒)</param>
        /// <param name="encoding">默认UTF8</param>
        /// <returns></returns>
        public static string HttpPost(string url, string postData, Dictionary<string, string> headers = null, string contentType = null, int timeout = 0, Encoding encoding = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.MaxResponseContentBufferSize = 1024 * 1024 * 10;

                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                if (timeout > 0)
                {
                    client.Timeout = new TimeSpan(0, 0, timeout);
                }
                using (HttpContent content = new StringContent(postData ?? "", encoding ?? Encoding.UTF8))
                {
                    if (contentType != null)
                    {
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                    }
                    using (HttpResponseMessage responseMessage = client.PostAsync(url, content).Result)
                    {
                        Byte[] resultBytes = responseMessage.Content.ReadAsByteArrayAsync().Result;
                        return Encoding.UTF8.GetString(resultBytes);
                    }
                }
            }
        }

        /// <summary>
        /// 异步POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <param name="timeout">请求响应超时时间，单位/s(默认100秒)</param>
        /// <param name="encoding">默认UTF8</param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string url, string postData, Dictionary<string, string> headers = null, string contentType = null, int timeout = 0, Encoding encoding = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.MaxResponseContentBufferSize = 1024 * 1024 * 10;
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                if (timeout > 0)
                {
                    client.Timeout = new TimeSpan(0, 0, timeout);
                }
                using (HttpContent content = new StringContent(postData ?? "", encoding ?? Encoding.UTF8))
                {
                    if (contentType != null)
                    {
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                    }
                    using (HttpResponseMessage responseMessage = await client.PostAsync(url, content))
                    {
                        Byte[] resultBytes = await responseMessage.Content.ReadAsByteArrayAsync();
                        return Encoding.UTF8.GetString(resultBytes);
                    }
                }
            }
        }

        #endregion
    }
}