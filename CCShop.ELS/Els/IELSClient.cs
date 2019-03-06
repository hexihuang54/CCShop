using System.Collections.Generic;

namespace CCShop.ELS
{
    public interface IELSClient
    {
        /// <summary>
        /// 创建/更新索引实体(T泛型对象)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="indexname"></param>
        /// <param name="strdoctype"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Index<T>(T model, string indexName, string strDocType, int id) where T : class;

        /// <summary>
        /// 创建/更新索引实体(object对象)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="indexname"></param>
        /// <param name="strdoctype"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Index(object data, string indexName, string strDocType, int id);

        /// <summary>
        /// 根据查询条件、排序条件获取索引实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyword"></param>
        /// <param name="orderbys"></param>
        /// <param name="indexname"></param>
        /// <param name="strdoctype"></param>
        /// <returns></returns>
        List<T> GetEntities<T>(string keyWord, string[] orderBys = null, string indexName = "", string strdoctype = "") where T : class;

        /// <summary>
        /// 判断指定索引是否存在
        /// </summary>
        /// <param name="indexname"></param>
        /// <returns></returns>
        bool ExistsIndex(string indexName);

        /// <summary>
        /// 删除指定索引
        /// </summary>
        /// <param name="indexname"></param>
        /// <returns></returns>
        bool DeleteIndex(string indexName);

        /// <summary>
        /// 根据查询条件、排序条件分页获取索引实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="row"></param>
        /// <param name="total"></param>
        /// <param name="keyword"></param>
        /// <param name="orderbys"></param>
        /// <param name="indexname"></param>
        /// <param name="strdoctype"></param>
        /// <returns></returns>
        List<T> GetPageEntities<T>(int page, int row, out int total, string keyword = "", string[] fields = null, string[] orderbys = null, string indexname = "", string strdoctype = "") where T : class;

        /// <summary>
        /// 删除指定索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="indexname"></param>
        /// <param name="strdoctype"></param>
        /// <returns></returns>
        bool Delete<T>(int id, string indexName, string strdoctype = "") where T : class;
    }
}
