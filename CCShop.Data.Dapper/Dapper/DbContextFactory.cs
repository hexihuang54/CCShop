using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace CCShop.Data.Dapper
{
    public abstract class DbContextFactory
    {
        protected DbContextFactory() { }
        public virtual DbConnection CreateConnection()
        {
            return new SqlConnection();
        }
    }

    public class SqlDbContextFactory : DbContextFactory
    {
        public override DbConnection CreateConnection()
        {
            return new SqlConnection();
        }
    }

    public class DbContextFactories
    {
        public static DbContextFactory GetFactory(string providername)
        {
            if (string.IsNullOrEmpty(providername))
            {
                throw new NoNullAllowedException();
            }

            switch (providername)
            {
                case "System.Data.SqlClient": return new SqlDbContextFactory();
                default: return null;
            }
        }
    }


}
