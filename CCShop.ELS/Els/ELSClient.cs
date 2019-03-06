using Nest;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CCShop.ELS
{
    public class ELSClient : IELSClient
    {
        private IElasticClient mElasticClent = null;

        private string mIndexName = string.Empty;
        private string mStrDocType = string.Empty;


        internal ELSClient(IConnectionSettingsValues conSettingValues)
        {
            this.mElasticClent = new ElasticClient(conSettingValues);
        }

        internal ELSClient(IConnectionSettingsValues conSettingValues, string indexName, string strDocType)
        {
            this.mIndexName = indexName;
            this.mStrDocType = strDocType;
            this.mElasticClent = new ElasticClient(conSettingValues);
        }

        /// <summary>
        /// 删除指定索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="indexname"></param>
        /// <param name="strdoctype"></param>
        /// <returns></returns>
        public bool Delete<T>(int id, string indexname = "", string strdoctype = "") where T : class
        {
            if (string.IsNullOrWhiteSpace(indexname))
            {
                indexname = this.mIndexName;
            }
            if (string.IsNullOrWhiteSpace(strdoctype))
            {
                strdoctype = this.mStrDocType;
            }
            return this.mElasticClent.Delete<T>(id, idx => idx.Index(indexname).Type(strdoctype)).IsValid;
        }

        /// <summary>
        /// 删除指定索引
        /// </summary>
        /// <param name="indexname"></param>
        /// <returns></returns>
        public bool DeleteIndex(string indexname)
        {
            return this.mElasticClent.DeleteIndex(indexname).IsValid;
        }

        /// <summary>
        /// 判断指定索引是否存在
        /// </summary>
        /// <param name="indexname"></param>
        /// <returns></returns>
        public bool ExistsIndex(string indexname)
        {
            return this.mElasticClent.IndexExists(indexname).Exists;
        }

        /// <summary>
        /// 根据查询条件、排序条件获取索引实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyword"></param>
        /// <param name="orderbys"></param>
        /// <param name="indexname"></param>
        /// <param name="strdoctype"></param>
        /// <returns></returns>
        public List<T> GetEntities<T>(string keyword, string[] orderbys = null, string indexname = "", string strdoctype = "") where T : class
        {
            if (string.IsNullOrWhiteSpace(indexname))
            {
                indexname = this.mIndexName;
            }
            if (string.IsNullOrWhiteSpace(strdoctype))
            {
                strdoctype = this.mStrDocType;
            }

            SearchRequest sr = new SearchRequest(indexname, strdoctype);

            RegexpQuery rq = new RegexpQuery();
            rq.Value = string.Format("*{0}*", keyword);
            rq.MaximumDeterminizedStates = 20000;
            sr.Query = rq;

            if (orderbys != null)
            {
                for (int i = 0; i < orderbys.Length; i++)
                {
                    string[] sortArry = orderbys[0].Split(':');
                    string filed = sortArry[0];
                    string order = sortArry[1];
                    ISort sort = new SortField { Field = filed, Order = (order.ToLower() == "asc" ? SortOrder.Ascending : SortOrder.Descending) };
                    sr.Sort = new List<ISort>();
                    sr.Sort.Add(sort);
                }
            }

            var searchResults = this.mElasticClent.Search<T>(sr);

            return searchResults.Documents.ToList();
        }

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
        public List<T> GetPageEntities<T>(int page, int row, out int total, string keyword = "", string[] fields = null, string[] orderbys = null, string indexname = "", string strdoctype = "") where T : class
        {
            total = 0;

            if (string.IsNullOrWhiteSpace(indexname))
            {
                indexname = this.mIndexName;
            }
            if (string.IsNullOrWhiteSpace(strdoctype))
            {
                strdoctype = this.mStrDocType;
            }

            var countRequest = new CountRequest(indexname, strdoctype);
            var searchRequest = new SearchRequest(indexname, strdoctype);

            //RegexpQuery rq = new RegexpQuery();
            //rq.Field = "name";
            //rq.Value = string.Format(".*{0}.*", keyword);
            //rq.MaximumDeterminizedStates = 20000;
            //searchRequest.Query = rq;

            var wholeKeyword = keyword;
            keyword = String.Format("*{0}*", keyword);
            QueryContainer query = new QueryStringQuery() { Query = keyword, DefaultOperator = Operator.And };
            if (!string.IsNullOrEmpty(wholeKeyword))
            {
                QueryContainer wholeWordQuery = new QueryStringQuery() { Query = wholeKeyword };
                query = query || wholeWordQuery;
            }

            searchRequest.Query = query;
            countRequest.Query = searchRequest.Query;

            total = (int)this.mElasticClent.Count<T>(countRequest).Count;

            searchRequest.From = (page - 1) * row;
            searchRequest.Size = row;

            if (fields != null)
            {
                searchRequest.Source = new SourceFilter()
                {
                    Includes = fields
                };
            }

            if (orderbys != null)
            {
                searchRequest.Sort = new List<ISort>();
                for (int i = 0; i < orderbys.Length; i++)
                {
                    string[] sortArry = orderbys[0].Split(':');
                    string filed = sortArry[0];
                    string order = sortArry[1];
                    ISort sort = new SortField { Field = filed, Order = (order.ToLower() == "asc" ? SortOrder.Ascending : SortOrder.Descending) };
                    searchRequest.Sort.Add(sort);
                }
            }

            var searchResults = this.mElasticClent.Search<T>(searchRequest);

            return searchResults.Documents.ToList<T>();
        }

        /// <summary>
        /// 创建/更新索引实体(object对象)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="indexname"></param>
        /// <param name="strdoctype"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Index(object data, string indexname, string strdoctype, int id)
        {
            try
            {
                var index = this.mElasticClent.Index(data, i => i.Index(indexname).Type(strdoctype).Id(id));
                return index.Created;
            }
            catch (Exception ex)
            {
                //Log.Err(ex);
            }
            return false;
        }

        /// <summary>
        /// 创建/更新索引实体(T泛型对象)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="indexname"></param>
        /// <param name="strdoctype"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Index<T>(T data, string indexname, string strdoctype, int id) where T : class
        {
            try
            {
                var index = this.mElasticClent.Index(data, i => i.Index(indexname).Type(strdoctype).Id(id));
                return index.Created;
            }
            catch (Exception ex)
            {
                // Log.Err(ex);
            }
            return false;
        }



    }
}
