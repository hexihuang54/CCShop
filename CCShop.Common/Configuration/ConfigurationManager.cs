using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace CCShop.Common.Configuration
{
    public class ConfigurationManager
    {
        private static IConfiguration config = null;

        static ConfigurationManager()
        {
            try
            {
                config = WebSiteSdk.GetWebSiteConfigBaseProxyClient("WebSiteConfig");
            }
            catch (System.Exception)
            {
            }
        }

        public static IConfiguration AppSettings
        {
            get
            {
                return config;
            }
        }

        public static string Get(string key)
        {
            return config[key];
        }

    }


    public class WebSiteSdk
    {
        private static readonly Dictionary<string, IConfiguration> Map = new Dictionary<string, IConfiguration>();

        public static void AddClientManager(IConfiguration proxy)
        {
            if (Map != null)
            {
                Map.Add("WebSiteConfig", proxy);
            }
        }

        public static IConfiguration GetWebSiteConfigBaseProxyClient(string key)
        {
            return Map[key];
        }
    }
}
