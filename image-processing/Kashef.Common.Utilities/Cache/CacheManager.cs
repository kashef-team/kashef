using Kashef.Common.Utilities.Cache; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.Cache
{
    public static class GenericCacheManager
    {
        private static ICacheManager _cacheManager;

        private static ICacheManager CacheManager
        {
            get
            {
                if (null == _cacheManager)
                {
                    _cacheManager = new MemoryCacheManager();
                }
                return _cacheManager;
            }
        }
         
        /// <summary>
        /// Cach data from two days..
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="key"></param>
        public static void AddCache<T>(T item, string key)
        {
            //Create Cache Configuration Item using the key..
            CacheItemConfig cacheItemConfig = new CacheItemConfig(new CacheKey(key), new TimeSpan(2, 0, 0, 0));

            CacheManager.Add(cacheItemConfig, item);
        }

        public static T GetCache<T>(string key)
        {
            //Create Cache Configuration Item using the key..
            CacheItemConfig cacheItemConfig = new CacheItemConfig(new CacheKey(key));

            //Get Cached Data...
            T cachedData;
            CacheManager.Get<T>(cacheItemConfig, out cachedData);
            return cachedData;
        }

        public static void UpdateCache<T>(T item, string key)
        { 
            //Remove Old Cache...
            CacheManager.Remove(new CacheKey(key));

            //Create New Cache
            AddCache<T>(item, key);
        }
    }
}
