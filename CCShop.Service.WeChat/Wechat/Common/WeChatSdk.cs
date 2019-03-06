using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace CCShop.Service.WeChat.Common
{
    public class WeChatSdk
    {
        private static readonly Dictionary<string, IConfigurationBuilder> Map = new Dictionary<string, IConfigurationBuilder>();

        public static void AddClientManager(IConfigurationBuilder proxy)
        {
            if (Map != null )
            {
                Map.Add("WeChatConfig", proxy);
            }
        }

        public static IConfigurationBuilder GetWeChatConfigBaseProxyClient(string key)
        {
            return Map[key];
        }

    }

}
