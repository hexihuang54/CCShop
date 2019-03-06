using System;
using System.Collections.Generic;
using System.Text;

namespace CCShop.Data.Dapper
{
    public class DataBaseProxy : IDataBaseProxy
    {
        private IDataBaseClient _DataBaseClient = null;
        private string _DbConnectionStr = string.Empty;
        private string _ProviderName = "System.Data.SqlClient";

        public DataBaseProxy(ConnectionConfig config)
        {
            this._DataBaseClient = new DataBaseClient(config.ProviderName);
            this._DbConnectionStr = config.DbConnectionStr;
            this._ProviderName = config.ProviderName;
        }

        public IDataClient GetDapperDataClient()
        {
            var dbConnectionFactory = this._DataBaseClient.CreateDapperDbFactory();
            return new DataClient(dbConnectionFactory, _DbConnectionStr);
        }
    }
}
