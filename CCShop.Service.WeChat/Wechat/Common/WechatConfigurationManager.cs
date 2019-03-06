using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CCShop.Service.WeChat.Common
{
    public class WechatConfigurationManager
    {
        private static IConfigurationRoot config = null;
        static WechatConfigurationManager()
        {
            try
            {
                var builder = WeChatSdk.GetWeChatConfigBaseProxyClient("WeChatConfig");

                if (builder == null)
                {
                    builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("wechatsetting.json");
                }
                config = builder.Build();
            }
            catch (Exception)
            {
            }
        }

        public static IConfigurationRoot AppSettings
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
}
