using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CCShop.Data.Dapper
{
    public class DataContext
    {
        [ThreadStatic]
        private IDataBaseProxy dataBaseProxy = null;

        public DataContext(string key)
        {
            dataBaseProxy = DataDapperSdk.GetDapperDataBaseClient(key);
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
            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.ExecuteScalar(sql, param, tran, type);
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
            //if (tran == null)
            //{
            //    return dbConnection.Execute(sql, param);
            //}
            //else
            //{
            //    return tran.Connection.Execute(sql, param, tran);
            //}
            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.ExecuteSql(sql, param, tran);
            }
        }

        /// <summary>
        /// 执行sql语句，返回List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pamram"></param>
        /// <returns></returns>
        public IList<T> QueryData<T>(string sql, object param = null, IDbTransaction tran = null, CommandType type = CommandType.Text)
        {
            //if (tran == null)
            //{
            //    return dbConnection.Query<T>(sql, pamram, tran, true, null, type).ToList();
            //}
            //else
            //{
            //    return tran.Connection.Query<T>(sql, pamram, tran, true, null, type).ToList();
            //}
            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.QueryData<T>(sql, param, tran, type);
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
            //IDataReader data;

            //if (tran == null)
            //{
            //    data = dbConnection.ExecuteReader(sql, param, tran, null, type);
            //}
            //else
            //{
            //    data = tran.Connection.ExecuteReader(sql, param, tran, null, type);
            //}

            //var dt = new DataTable();
            //dt.Load(data);
            //return dt;

            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.QueryData(sql, param, tran, type);
            }

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
            //var args = new DynamicParameters(param);
            //args.Add("@total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            //var data = dbConnection.ExecuteReader(sql, args, null, null, type);
            //var dt = new DataTable();
            //dt.Load(data);
            //total = args.Get<int>("@total");
            //return dt;


            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.Page(out total, sql, param, type);
            }

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
            //var args = new DynamicParameters(param);
            //args.Add("@total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            //var data = dbConnection.ExecuteReader(sql, args, null, null, type);

            //var dt = new DataTable();
            //dt.Load(data);

            //string tempName = string.Empty;
            //var list = new List<T>();

            //foreach (DataRow dr in dt.Rows)
            //{
            //    T t = new T();
            //    PropertyInfo[] propertys = t.GetType().GetProperties();// 获得此模型的公共属性
            //    foreach (PropertyInfo pi in propertys)
            //    {
            //        tempName = pi.Name;
            //        if (dt.Columns.Contains(tempName))
            //        {
            //            if (!pi.CanWrite) continue;
            //            object value = dr[tempName];
            //            if (value != DBNull.Value)
            //                pi.SetValue(t, value, null);
            //        }
            //    }
            //    list.Add(t);
            //}

            //total = args.Get<int>("@total");

            //return list;

            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.Page<T>(out total, sql, param, type);
            }

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
            //var args = new DynamicParameters(param);
            //args.Add("@RecordCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            //var data = dbConnection.ExecuteReader("dbo.ProcGetPageData", args, null, null, CommandType.StoredProcedure);

            //var dt = new DataTable();
            //dt.Load(data);

            //string tempName = string.Empty;
            //var list = new List<T>();

            //foreach (DataRow dr in dt.Rows)
            //{
            //    T t = new T();
            //    PropertyInfo[] propertys = t.GetType().GetProperties();// 获得此模型的公共属性
            //    foreach (PropertyInfo pi in propertys)
            //    {
            //        tempName = pi.Name;
            //        if (dt.Columns.Contains(tempName))
            //        {
            //            if (!pi.CanWrite) continue;
            //            object value = dr[tempName];
            //            if (value != DBNull.Value)
            //                pi.SetValue(t, value, null);
            //        }
            //    }
            //    list.Add(t);
            //}

            //total = args.Get<int>("@RecordCount");

            //return list;

            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.Page<T>(out total, param);
            }
        }

        /// <summary>
        /// 执行分页存储过程,返回DataTable集合数据
        /// </summary>
        /// <param name="total"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public DataTable Page(out int total, object param = null)
        {
            //var args = new DynamicParameters(param);
            //args.Add("@RecordCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            //var data = dbConnection.ExecuteReader("dbo.ProcGetPageData", args, null, null, CommandType.StoredProcedure);

            //var dt = new DataTable();
            //dt.Load(data);

            //total = args.Get<int>("@RecordCount");

            //return dt;

            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.Page(out total, param);
            }

        }

        /// <summary>
        /// 获取所有数据,返回List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> GetList<T>() where T : class
        {
            //return dbConnection.GetList<T>().ToList();
            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.GetList<T>();
            }
        }

        /// <summary>
        /// 获取所有数据,返回List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IList<T> GetList<T>(object predicate = null) where T : class
        {
            //return dbConnection.GetList<T>(predicate).ToList();
            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.GetList<T>(predicate);
            }
        }

        /// <summary>
        /// 获取所有数据,返回DataTable集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DataTable GetTable<T>() where T : class
        {
            //return (DataTable)dbConnection.GetList<T>().AsQueryable();

            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return (DataTable)dataClient.GetList<T>().AsQueryable();
            }
        }

        /// <summary>
        /// 根据主键ID,返回实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetData<T>(int id) where T : class
        {
            //var data = dbConnection.Get<T>(id);
            //return data;

            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.GetData<T>(id);
            }
        }

        public T GetData<T>(long id) where T : class
        {
            //var data = dbConnection.Get<T>(id);
            //return data;

            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.GetData<T>(id);
            }
        }

        public T GetData<T>(string sqlWhere) where T : class
        {
            //var data = dbConnection.Get<T>(id);
            //return data;

            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.GetData<T>(sqlWhere);
            }
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
            //if (tran == null)
            //{
            //    return dbConnection.Insert(entity, dbConnection.BeginTransaction());
            //}
            //else
            //{
            //    return tran.Connection.Insert(entity, tran);
            //}

            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.Insert<T>(entity, tran);
            }
        }

        /// <summary>
        /// 添加数据方法,返回主键ID,且支持事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public long InsertBigInt<T>(T entity, IDbTransaction tran = null) where T : class
        {
            //if (tran == null)
            //{
            //    return dbConnection.Insert(entity, dbConnection.BeginTransaction());
            //}
            //else
            //{
            //    return tran.Connection.Insert(entity, tran);
            //}

            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.InsertBigInt<T>(entity, tran);
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
            //if (tran == null)
            //{
            //    return dbConnection.Insert(entity, dbConnection.BeginTransaction()) > 0;
            //}
            //else
            //{
            //    return tran.Connection.Insert(entity, tran) > 0;
            //}

            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.InsertData<T>(entity, tran);
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

            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                dataClient.InsertBatchTran<T>(entityList, tran);
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
            //if (tran == null)
            //{
            //    return dbConnection.Update(entity);
            //}
            //else
            //{
            //    return tran.Connection.Update(entity, tran);
            //}

            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.Update<T>(entity, tran);
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
            //if (tran == null)
            //{
            //    return dbConnection.Delete<T>(entity, dbConnection.BeginTransaction());
            //}
            //else
            //{
            //    return tran.Connection.Delete(entity, tran);
            //}

            using (var dataClient = dataBaseProxy.GetDapperDataClient())
            {
                return dataClient.Delete<T>(entity, tran);
            }

        }

    }
}
