using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace CCShop.Data.Dapper
{
    /// <summary>
    /// Dapper数据库操作接口
    /// </summary>
    public interface IDataClient : IDisposable
    {
        IDbTransaction Begin(IsolationLevel isolation = IsolationLevel.ReadCommitted);

        void Commit();

        void Rollback();


        /// <summary>
        /// 判断是否存在字段
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        bool readerExists(IDataReader dr, string columnName);


        /// <summary>
        /// 执行sql语句,返回第一行第一列的值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="tran"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        object ExecuteScalar(string sql, object param = null, IDbTransaction tran = null, CommandType type = CommandType.Text);


        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pamram"></param>
        /// <returns></returns>
        int ExecuteSql(string sql, object param = null, IDbTransaction tran = null);

        /// <summary>
        /// 执行sql语句，返回List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pamram"></param>
        /// <returns></returns>
        IList<T> QueryData<T>(string sql, object pamram = null, IDbTransaction tran = null, CommandType type = CommandType.Text);


        /// <summary>
        /// 执行sql语句，返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pamram"></param>
        /// <returns></returns>
        DataTable QueryData(string sql, object param = null, IDbTransaction tran = null, CommandType type = CommandType.Text);


        /// <summary>
        /// 执行sql分页语法,返回DataTable集合数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="type"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        DataTable Page(out int total, string sql, object param = null, CommandType type = CommandType.Text);

        /// <summary>
        /// 执行sql分页语法,返回List实体集合数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="type"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        List<T> Page<T>(out int total, string sql, object param = null, CommandType type = CommandType.Text) where T : new();


        /// <summary>
        /// 执行分页存储过程,返回List实体集合数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="total"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        List<T> Page<T>(out int total, object param = null) where T : new();


        /// <summary>
        /// 执行分页存储过程,返回DataTable集合数据
        /// </summary>
        /// <param name="total"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        DataTable Page(out int total, object param = null);


        /// <summary>
        /// 获取所有数据,返回List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IList<T> GetList<T>() where T : class;


        /// <summary>
        /// 获取所有数据,返回List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IList<T> GetList<T>(object predicate = null) where T : class;


        /// <summary>
        /// 获取所有数据,返回DataTable集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        DataTable GetTable<T>() where T : class;


        /// <summary>
        /// 根据主键ID,返回实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetData<T>(int id) where T : class;

        T GetData<T>(long id) where T : class;

        T GetData<T>(string sqlwhere) where T : class;

        /// <summary>
        /// 添加数据方法,返回主键ID,且支持事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        int Insert<T>(T entity, IDbTransaction tran = null) where T : class;

        /// <summary>
        /// 添加数据方法,返回主键ID,且支持事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        long InsertBigInt<T>(T entity, IDbTransaction tran = null) where T : class;


        /// <summary>
        /// 添加数据方法,返回是否成功状态,且支持事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        bool InsertData<T>(T entity, IDbTransaction tran = null) where T : class;


        /// <summary>
        /// 批量添加实体集合,返回void空结果,且支持事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityList"></param>
        /// <param name="tran"></param>
        void InsertBatchTran<T>(IEnumerable<T> entityList, IDbTransaction tran) where T : class;


        /// <summary>
        /// 修改数据库实体对象方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        bool Update<T>(T entity, IDbTransaction tran = null) where T : class;


        /// <summary>
        /// 删除数据库记录方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        bool Delete<T>(T entity, IDbTransaction tran = null) where T : class;

        bool Delete<T>(string sqlwhere, IDbTransaction tran = null) where T : class;

    }


    /// <summary>
    /// Dapper数据库访问代理接口
    /// </summary>
    public interface IDataBaseProxy
    {
        IDataClient GetDapperDataClient();
    }

    /// <summary>
    /// Dapper数据库访问对象创建接口
    /// </summary>
    public interface IDataBaseClient
    {
        DbContextFactory CreateDapperDbFactory();
    }



}
