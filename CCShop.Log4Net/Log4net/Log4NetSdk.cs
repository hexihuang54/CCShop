using CCShop.Common.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace CCShop.Log4Net
{
    public class Log4NetSdk
    {
        private static readonly Dictionary<string, ILogBaseProxy> Map = new Dictionary<string, ILogBaseProxy>();

        public static void AddClientManager(ILogBaseProxy proxy, string key = "Log4net")
        {
            if (Map != null && !Map.ContainsKey(key))
            {
                Map.Add(key, proxy);
            }
        }

        public static ILogBaseProxy GetLogBaseProxyClient(string key)
        {
            return Map[key];
        }
    }



    public class Log
    {
        [ThreadStatic]
        public static string ContextID = string.Empty;
        [ThreadStatic]
        public static string ContextName = string.Empty;
        [ThreadStatic]
        public static string IpAddress = string.Empty;

        private readonly static ILogClient log = Log4NetSdk.GetLogBaseProxyClient("Log4net").CreateLogClient();

        private static string format(string action)
        {
            var prid = Process.GetCurrentProcess().Id.ToString();
            var thid = Thread.CurrentThread.ManagedThreadId.ToString();

            var info = new LogItemInfo
            {
                action = action,
                contextid = ContextID,
                contextname = ContextName,
                processid = prid,
                threadid = thid,
                ipaddress = IpAddress
            };

            return JsonConvert.SerializeObject(info);
        }


        /// <summary>
        /// 添加错误日志信息
        /// </summary>
        /// <param name="ex"></param>
        public static void Err(Exception ex, string action = "")
        {
            if (ConfigurationManager.AppSettings["LogInfoState"].ToString() == "1")
            {
                ErrData(ex, action);
            }
        }


        /// <summary>
        /// 添加消息日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Info(string msg, string action = "")
        {
            if (ConfigurationManager.AppSettings["LogErrState"].ToString() == "1")
            {
                InfoData(msg, action);
            }
        }


        private static void ErrData(Exception ex, string action = "")
        {
            string message = string.Format("|head| {0} |info| {1} |", format(action), ex.Message);
            log.Err(ex, message);
        }

        public static void InfoData(string msg, string action = "")
        {
            string message = string.Format("|head| {0} |info| {1} |", format(action), msg);
            log.Info(message);
        }
    }


    public class LogItemInfo
    {
        public string contextid { get; set; }

        public string contextname { get; set; }

        public string processid { get; set; }

        public string action { get; set; }

        public string ipaddress { get; set; }

        public string threadid { get; set; }
    }
}
