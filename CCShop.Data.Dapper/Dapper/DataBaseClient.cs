using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace CCShop.Data.Dapper
{
    public class DataBaseClient : IDataBaseClient
    {
        private DbContextFactory _dbContextFactory = null;

        public DataBaseClient(string providername)
        {
            _dbContextFactory = DbContextFactories.GetFactory(providername);
        }

        public DbContextFactory CreateDapperDbFactory()
        {
            return _dbContextFactory;
        }
    }
}
