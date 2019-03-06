using System.Collections.Generic;

namespace CCShop.ELS
{
    public class ELSSdk
    {
        private static readonly Dictionary<string, IELSBaseProxy> Map = new Dictionary<string, IELSBaseProxy>();

        public static void AddClientManager(IELSBaseProxy proxy, string key = "")
        {
            if (Map != null && !Map.ContainsKey(key))
            {
                Map.Add(key, proxy);
            }
        }

        public static IELSBaseProxy GetElsProxyClient(string key)
        {
            return Map[key];
        }

        public static IELSBaseProxy GetElsSearchClient(string elsuri)
        {
            return (IELSBaseProxy)new ELSBaseProxy(elsuri);
        }
    }
}
