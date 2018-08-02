using System;

namespace Kashef.Common.Utilities.Cache
{
    /// <summary>
    /// A cache item configuration
    /// </summary>
    public class CacheItemConfig
    {
        #region -- Local Variables --
        private CacheKey m_cacheKey;
        private TimeSpan m_expirationTime;
        #endregion

        #region -- Properties --
        
        /// <summary>
        /// Get the associated cached key
        /// </summary>
        public CacheKey CacheKey
        {
            get
            {
                return m_cacheKey;
            }
        }        

        /// <summary>
        /// Get the associted expiration time
        /// </summary>
        public TimeSpan ExpirationTime
        {
            get
            {
                return m_expirationTime;
            }
        }

        #endregion

        #region -- Constructor(s) --

        /// <summary>
        /// Create a new instance of cache item
        /// </summary>
        /// <param name="cacheKey">The cached key</param>
        public CacheItemConfig(CacheKey cacheKey)
            : this(cacheKey, new TimeSpan(0, 0, 10))
        {
        }

        /// <summary>
        /// Create a new instance of cache item
        /// </summary>
        /// <param name="cacheKey">The cached key</param>
        /// <param name="expirationTime">Associated expiration time</param>
        public CacheItemConfig(CacheKey cacheKey, TimeSpan expirationTime)
        {
            if (cacheKey == (CacheKey)null)
                throw new ArgumentNullException("cacheKey");

            m_cacheKey = cacheKey;
            m_expirationTime = expirationTime;

        }

        #endregion

    }
}
