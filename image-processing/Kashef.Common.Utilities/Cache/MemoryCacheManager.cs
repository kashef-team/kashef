using System;
using System.Runtime.Caching;

namespace Kashef.Common.Utilities.Cache
{
    /// <summary>
    /// Cache manager implementation with .Net im memory cache.
    /// </summary>
    /// <remarks>It is recomended using "singleton" life time in the selected IoC.</remarks>
    public sealed class MemoryCacheManager : ICacheManager, IDisposable
    {
        #region -- Local Variables --
        private MemoryCache m_CacheFactory;
        #endregion

        #region -- Constructor --

        /// <summary>
        /// Create a new constructor of cache manager. 
        /// It is recomended using "singleton" life time in the selected IoC.
        /// </summary>
        public MemoryCacheManager()
        {
            //configuration for this cache factory is delegated in application configuration file
            m_CacheFactory = MemoryCache.Default;
        }

        #endregion

        #region --  Public Methods --

        /// <summary>
        /// <see cref="M:Kashef.Common.Utilities.Cache.Get{TResult}"/>
        /// </summary>
        /// <typeparam name="TResult"><see cref="M:Kashef.Common.Utilities.Cache.Get{TResult}"/></typeparam>
        /// <param name="cacheItemConfig"><see cref="M:Kashef.Common.Utilities.Cache.Get{TResult}"/></param>
        /// <param name="result"><see cref="M:Kashef.Common.Utilities.Cache.Get{TResult}"/>></param>
        /// <returns><see cref="M:Kashef.Common.Utilities.Cache.Get{TResult}"/></returns>
        public bool Get<TResult>(CacheItemConfig cacheItemConfig, out TResult result)
        {
            if (cacheItemConfig != null)
            {
                string cacheKey = cacheItemConfig.CacheKey.GetCacheKey();

                //get object from cache and check if exists
                object cachedItem = this.m_CacheFactory.Get(cacheKey);

                if (cachedItem != null)
                {
                    result = (TResult)cachedItem;

                    return true;
                }
                else
                {
                    result = default(TResult);

                    return false;
                }
            }
            else
                throw new ArgumentNullException("cacheItem");
        }

        /// <summary>
        /// <see cref="M:Kashef.Common.Utilities.Cache..Get{TResult}"/>
        /// </summary>
        /// <param name="cacheItemConfig"><see cref="M:Kashef.Common.Utilities.Cache.Caching.Get{TResult}"/></param>
        /// <param name="value"><see cref="M:Kashef.Common.Utilities.Cache.Get{TResult}"/></param>
        public void Add(CacheItemConfig cacheItemConfig, object value)
        {
            if (value != null && cacheItemConfig != null)
            {
                string cachekey = cacheItemConfig.CacheKey.GetCacheKey();
                TimeSpan expirationTime = cacheItemConfig.ExpirationTime;
                this.m_CacheFactory.Add(cachekey, value, new DateTimeOffset(DateTime.Now.AddTicks(expirationTime.Ticks)));
            }
            else if (cacheItemConfig == null)
            {
                throw new ArgumentNullException(Properties.Resources.Exception_CacheItemKeyNull);
            }

        }

        /// <summary>
        /// <see cref="M:Kashef.Common.Utilities.Cache.Caching.Remove"/>
        /// </summary>
        /// <param name="cacheKey"><see cref="M:Kashef.Common.Utilities.Cache.Remove"/></param>
        public bool Remove(CacheKey cacheKey)
        {
            if (cacheKey != null)
            {
                return this.m_CacheFactory.Remove(cacheKey.GetCacheKey()) != null;
            }
            else
                return false;
        }

        #endregion

        #region -- IDisposable Members --

        /// <summary>
        /// Dispose in memory cache.
        /// </summary>
        public void Dispose()
        {
            if (m_CacheFactory != null)
                m_CacheFactory.Dispose();
        }

        #endregion
    }
}
