using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using DapperExtensions;
using DapperExtensions.Sql;
using System.Data.SqlClient;

namespace CCShop.Data.Dapper
{
    public class DataClient : IDataClient
    {
        private IDbConnection dbConnection = null;
        private IDbTransaction transaction = null;

        private object _lock = new object();

        public DataClient(ConnectionConfig config)
        {
            if (this.dbConnection == null)
            {
                this.dbConnection = DbContextFactories.GetFactory(config.ProviderName).CreateConnection();
                this.dbConnection.ConnectionString = config.DbConnectionStr;
                this.dbConnection.Open();
            }

            if (this.dbConnection.State == ConnectionState.Closed)
            {
                lock (_lock)
                {
                    this.dbConnection.Open();
                }
            }
        }

        public DataClient(DbContextFactory factory, string dbConnectionstr)
        {
            //if (this.dbConnection == null)
            //{
            //    this.dbConnection = factory.CreateConnection();

            //    if (this.dbConnection != null)
            //    {
            //        this.dbConnection.ConnectionString = dbConnectionstr;
            //        this.dbConnection.Open();
            //    }

            //    if (this.dbConnection.State == ConnectionState.Closed)
            //    {
            //        this.dbConnection.Open();
            //    }
            //}
            try
            {

                if (factory != null)
                {
                    this.dbConnection = factory.CreateConnection();

                    if (this.dbConnection != null)
                    {
                        if (this.dbConnection != null)
                        {
                            this.dbConnection.ConnectionString = dbConnectionstr;
                            this.dbConnection.Open();
                        }
                    }
                }
                else
                {
                    this.dbConnection = DbContextFactories.GetFactory(new ConnectionConfig().ProviderName).CreateConnection();
                    if (this.dbConnection != null)
                    {
                        if (this.dbConnection != null)
                        {
                            this.dbConnection.ConnectionString = dbConnectionstr;
                            this.dbConnection.Open();
                        }
                    }
                }

            }
            catch (Exception e) 
            {
                Console.Write("Open--msg:" + e.Message + ",error:" + e.StackTrace);
            }
        }


        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="isolation"></param>
        /// <returns></returns>
        public IDbTransaction Begin(IsolationLevel isolation = IsolationLevel.ReadCommitted)
        {
            transaction = dbConnection.BeginTransaction(isolation);
            return transaction;
        }

        /// <summary>
        /// 事务提交
        /// </summary>
        public void Commit()
        {
            transaction.Commit();
            transaction = null;
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        public void Rollback()
        {
            transaction.Rollback();
            transaction.Dispose();
        }

        /// <summary>
        /// 释放连接池对象
        /// </summary>
        public void Dispose()
        {
            if (dbConnection != null)
            {
                try
                {
                    if (dbConnection.State != ConnectionState.Closed)
                    {
                        if (transaction != null)
                        {
                            transaction.Dispose();
                        }
                    }

                    dbConnection.Close();
                    dbConnection.Dispose();
                    dbConnection = null;
                    // GC.Collect();
                }
                catch (Exception e)
                {
                    Console.Write("Close--msg:" + e.Message + ",error:" + e.StackTrace);
                }
            }
        }

        /// <summary>
        /// 判断是否存在字段
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public bool readerExists(IDataReader dr, string columnName)
        {
            dr.GetSchemaTable().DefaultView.RowFilter = "ColumnName= '" + columnName + "'";
            return (dr.GetSchemaTable().DefaultView.Count > 0);
        }

        /// <summary>
        /// 执行sql语句,返回第一行第一列的值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="tran"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, object param = null, IDbTransaction tran = null, CommandType type = CommandType.Text)
        {
            if (tran == null)
            {
                return dbConnection.ExecuteScalar(sql, param, null, null, type);
            }
            else
            {
                return tran.Connection.ExecuteScalar(sql, param, tran);
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pamram"></param>
        /// <returns></returns>
        public int ExecuteSql(string sql, object param = null, IDbTransaction tran = null)
        {
            if (tran == null)
            {
                return dbConnection.Execute(sql, param);
            }
            else
            {
                return tran.Connection.Execute(sql, param, tran);
            }
        }

        /// <summary>
        /// 执行sql语句，返回List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pamram"></param>
        /// <returns></returns>
        public IList<T> QueryData<T>(string sql, object pamram = null, IDbTransaction tran = null, CommandType type = CommandType.Text)
        {
            if (tran == null)
            {
                return dbConnection.Query<T>(sql, pamram, tran, true, null, type).ToList();
            }
            else
            {
                return tran.Connection.Query<T>(sql, pamram, tran, true, null, type).ToList();
            }
        }

        /// <summary>
        /// 执行sql语句，返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pamram"></param>
        /// <returns></returns>
        public DataTable QueryData(string sql, object param = null, IDbTransaction tran = null, CommandType type = CommandType.Text)
        {
            IDataReader data;

            if (tran == null)
            {
                data = dbConnection.ExecuteReader(sql, param, tran, null, type);
            }
            else
            {
                data = tran.Connection.ExecuteReader(sql, param, tran, null, type);
            }

            var dt = new DataTable();
            dt.Load(data);


            return dt;

        }

        /// <summary>
        /// 执行sql分页语法,返回DataTable集合数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="type"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public DataTable Page(out int total, string sql, object param = null, CommandType type = CommandType.Text)
        {
            var args = new DynamicParameters(param);
            args.Add("@total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var data = dbConnection.ExecuteReader(sql, args, null, null, type);
            var dt = new DataTable();
            dt.Load(data);
            total = args.Get<int>("@total");
            return dt;
        }

        /// <summary>
        /// 执行sql分页语法,返回List实体集合数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="type"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<T> Page<T>(out int total, string sql, object param = null, CommandType type = CommandType.Text) where T : new()
        {
            var args = new DynamicParameters(param);
            args.Add("@total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var data = dbConnection.ExecuteReader(sql, args, null, null, type);

            var dt = new DataTable();
            dt.Load(data);

            string tempName = string.Empty;
            var list = new List<T>();

            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                PropertyInfo[] propertys = t.GetType().GetProperties();// 获得此模型的公共属性
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;
                    if (dt.Columns.Contains(tempName))
                    {
                        if (!pi.CanWrite) continue;
                        object value = dr[tempName];
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                list.Add(t);
            }

            total = args.Get<int>("@total");

            return list;
        }

        /// <summary>
        /// 执行分页存储过程,返回List实体集合数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="total"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<T> Page<T>(out int total, object param = null) where T : new()
        {
            var args = new DynamicParameters(param);
            args.Add("@RecordCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var data = dbConnection.ExecuteReader("dbo.ProcGetPageData", args, null, null, CommandType.StoredProcedure);

            var dt = new DataTable();
            dt.Load(data);

            string tempName = string.Empty;
            var list = new List<T>();

            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                PropertyInfo[] propertys = t.GetType().GetProperties();// 获得此模型的公共属性
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;
                    if (dt.Columns.Contains(tempName))
                    {
                        if (!pi.CanWrite) continue;
                        object value = dr[tempName];
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                list.Add(t);
            }

            total = args.Get<int>("@RecordCount");

            return list;
        }

        /// <summary>
        /// 执行分页存储过程,返回DataTable集合数据
        /// </summary>
        /// <param name="total"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public DataTable Page(out int total, object param = null)
        {
            var args = new DynamicParameters(param);
            args.Add("@RecordCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var data = dbConnection.ExecuteReader("dbo.ProcGetPageData", args, null, null, CommandType.StoredProcedure);

            var dt = new DataTable();
            dt.Load(data);

            total = args.Get<int>("@RecordCount");

            return dt;
        }

        /// <summary>
        /// 获取所有数据,返回List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> GetList<T>() where T : class
        {
            return dbConnection.GetList<T>().ToList();
        }

        /// <summary>
        /// 获取所有数据,返回List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IList<T> GetList<T>(object predicate = null) where T : class
        {
            return dbConnection.GetList<T>(predicate).ToList();
        }

        /// <summary>
        /// 获取所有数据,返回DataTable集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DataTable GetTable<T>() where T : class
        {
            return (DataTable)dbConnection.GetList<T>().AsQueryable();
        }

        /// <summary>
        /// 根据主键ID,返回实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetData<T>(int id) where T : class
        {
            var data = dbConnection.Get<T>(id);
            return data;
        }

        public T GetData<T>(long id) where T : class
        {
            var data = dbConnection.Get<T>(id);
            return data;
        }

        public T GetData<T>(string sqlwhere) where T : class
        {
            var t = Activator.CreateInstance<T>();
            var tablename = t.GetType().Name;
            var strsql = @" select * from " + tablename;
            if (!string.IsNullOrWhiteSpace(sqlwhere))
            {
                strsql += " where " + sqlwhere;
            }
            var data = dbConnection.QueryFirstOrDefault<T>(strsql);
            return data;
        }


        /// <summary>
        /// 添加数据方法,返回主键ID,且支持事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public int Insert<T>(T entity, IDbTransaction tran = null) where T : class
        {
            if (tran == null)
            {
                return dbConnection.Insert(entity, tran);
            }
            else
            {
                return tran.Connection.Insert(entity, tran);
            }
        }

        /// <summary>
        /// 添加数据方法,返回是否成功状态,且支持事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public bool InsertData<T>(T entity, IDbTransaction tran = null) where T : class
        {
            if (tran == null)
            {
                return dbConnection.Insert(entity, tran) > 0;
            }
            else
            {
                return tran.Connection.Insert(entity, tran) > 0;
            }
        }

        /// <summary>
        /// 批量添加实体集合,返回void空结果,且支持事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityList"></param>
        /// <param name="tran"></param>
        public void InsertBatchTran<T>(IEnumerable<T> entityList, IDbTransaction tran) where T : class
        {
            //var tblName = string.Format("dbo.{0}", typeof(T).Name);
            //var tranSaction = (SqlTransaction)tran;
            //using (var bulkCopy = new SqlBulkCopy(tran.Connection as SqlConnection, SqlBulkCopyOptions.TableLock, tranSaction))
            //{
            //    bulkCopy.BatchSize = entityList.Count();
            //    bulkCopy.DestinationTableName = tblName;
            //    var table = new DataTable();
            //    ISqlGenerator sqlGenerator = new SqlGeneratorImpl(new DapperExtensionsConfiguration());
            //    var classMap = sqlGenerator.Configuration.GetMap<T>();
            //    var props = classMap.Properties.Where(x => x.Ignored == false).ToArray();
            //    foreach (var propertyInfo in props)
            //    {
            //        bulkCopy.ColumnMappings.Add(propertyInfo.Name.ToLower(), propertyInfo.Name.ToLower());
            //        table.Columns.Add(propertyInfo.Name.ToLower(), Nullable.GetUnderlyingType(propertyInfo.PropertyInfo.PropertyType) ?? propertyInfo.PropertyInfo.PropertyType);
            //    }
            //    var values = new object[props.Count()];
            //    foreach (var itemm in entityList)
            //    {
            //        for (var i = 0; i < values.Length; i++)
            //        {
            //            values[i] = props[i].PropertyInfo.GetValue(itemm, null);
            //        }
            //        table.Rows.Add(values);
            //    }

            //    bulkCopy.WriteToServer(table);
            //}


            foreach (var itemm in entityList)
            {
                InsertData(itemm, tran);
            }
        }

        /// <summary>
        /// 修改数据库实体对象方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public bool Update<T>(T entity, IDbTransaction tran = null) where T : class
        {
            if (tran == null)
            {
                return dbConnection.Update(entity);
            }
            else
            {
                return tran.Connection.Update(entity, tran);
            }
        }

        /// <summary>
        /// 删除数据库记录方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public bool Delete<T>(T entity, IDbTransaction tran = null) where T : class
        {
            if (tran == null)
            {
                return dbConnection.Delete<T>(entity, tran);
            }
            else
            {
                return tran.Connection.Delete(entity, tran);
            }
        }

        public bool Delete<T>(string sqlwhere, IDbTransaction tran = null) where T : class
        {
            if (tran == null)
            {
                var t = Activator.CreateInstance<T>();
                var tablename = t.GetType().Name;
                var strsql = @" delete from " + tablename;
                if (!string.IsNullOrWhiteSpace(sqlwhere))
                {
                    strsql += " where " + sqlwhere;
                }
                return dbConnection.Execute(strsql) > 0;
            }
            else
            {
                var t = Activator.CreateInstance<T>();
                var tablename = t.GetType().Name;
                var strsql = @" delete from " + tablename;
                if (!string.IsNullOrWhiteSpace(sqlwhere))
                {
                    strsql += " where " + sqlwhere;
                }
                return dbConnection.Execute(strsql, null, tran) > 0;
            }
        }

        public long InsertBigInt<T>(T entity, IDbTransaction tran = null) where T : class
        {
            if (tran == null)
            {
                return dbConnection.Insert(entity, tran);
            }
            else
            {
                return tran.Connection.Insert(entity, tran);
            }
        }
    }
}
