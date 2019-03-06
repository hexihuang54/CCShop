using System;
using System.Collections.Generic;
using System.Text;

namespace CCShop.Data.Dapper
{
    public class DataDapperSdk
    {
        private static readonly Dictionary<string, IDataBaseProxy> Map = new Dictionary<string, IDataBaseProxy>();

        public static void AddClientManagerList(List<KeyValuePair<string, IDataBaseProxy>> clientMgrList)
        {
            if (clientMgrList != null && clientMgrList.Count > 0)
            {
                foreach (var item in clientMgrList)
                {
                    Map.Add(item.Key, item.Value);
                }
            }
        }

        public static void AddClientManager(IDataBaseProxy clientMgr, string key = "")
        {
            Map.Add(key, clientMgr);
        }

        public static IDataBaseProxy GetDapperDataBaseClient(string key)
        {
            if (Map.ContainsKey(key))
            {
                return Map[key];
            }
            return null;
        }

        public static IDataClient GetDapperDataClient(ConnectionConfig config)
        {
            return new DataClient(config);
        }
    }
}
