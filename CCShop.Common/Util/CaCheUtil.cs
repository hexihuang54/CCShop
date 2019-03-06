using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCShop.Common.Util
{
    public class CaCheUtil
    {

        public static IMemoryCache _memoryCache  ;

        /// <summary>
        /// 判断缓存中只否存在指定键值的缓存
        /// </summary>
        /// <param name="cacheKey">缓存键值</param>
        /// <returns>一个值，指示缓存是否存在</returns>
        public static bool IsCachedExist(string cacheKey)
        {
            if (cacheKey == null)
            {
                throw new ArgumentNullException();
            }
            object obj = null;
            return _memoryCache.TryGetValue(cacheKey, out obj);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheData"></param>
        /// <param name="cacheInterval">过期时间，单位：分钟</param>
        /// <returns></returns>
        public static bool InsertCache(string cacheKey, object cacheData, int cacheInterval)
        {
            if (cacheKey != null && cacheKey.Length != 0)
            {
                TimeSpan tspan = new TimeSpan(0, cacheInterval, 0);
                _memoryCache.Set(cacheKey, cacheData, new MemoryCacheEntryOptions().SetAbsoluteExpiration(tspan));

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 使用缓存依赖插入一个缓存项
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheData"></param>
        /// <param name="cacheDependency"></param>
        /// <returns></returns>
        public static bool InsertCache(string cacheKey, object cacheData, DateTime absoluteExpiration)
        {
            if (cacheKey != null && cacheKey.Length != 0)
            {
                _memoryCache.Set(cacheKey, cacheData, new MemoryCacheEntryOptions().SetAbsoluteExpiration(absoluteExpiration));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="cacheKey">缓存键值</param>
        /// <returns></returns>
        public static object GetCachedData(string cacheKey)
        {
            return _memoryCache.Get(cacheKey);
        }


        public  void Dispose()
        {
            if (_memoryCache != null)
                _memoryCache.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
