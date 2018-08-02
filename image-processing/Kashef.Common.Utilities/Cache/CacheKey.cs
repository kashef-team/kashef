using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kashef.Common.Utilities.Cache
{   
    /// <summary>
    /// Represent a cache key information for Cache manager
    /// </summary>
    public sealed class CacheKey
    {
        #region -- Local Variables --
        private string m_keyName;
        private Dictionary<string, string> m_cacheVaryParams;
        #endregion

        #region -- Properties --
        /// <summary>
        /// Get the key name for this cache key
        /// </summary>
        public string KeyName
        {
            get
            {
                return m_keyName;
            }
        }
        
        /// <summary>
        /// Get the name value collection of elements that vary the cache item
        /// </summary>
        public Dictionary<string, string> CacheVaryParams
        {
            get
            {
                return m_cacheVaryParams;
            }
        }
        #endregion

        #region -- Constructor(s) --

        /// <summary>
        /// Create a new instance of CacheKey
        /// </summary>
        /// <param name="keyName">the key name</param>
        public CacheKey(string keyName)
        {
            //check preconditions for this input params!
            if (String.IsNullOrEmpty(keyName) ||String.IsNullOrWhiteSpace(keyName))
            {
                throw new ArgumentNullException("keyName");
            }
            m_keyName = keyName;
        }

        /// <summary>
        /// Create a new instance of CacheKey
        /// </summary>
        /// <param name="keyName">The cache key name</param>
        /// <param name="varyParams">The vary params of this cache item</param>
        public CacheKey(string keyName, Dictionary<string, string> varyParams)
        {
            //check preconditions for this input params!
            if (String.IsNullOrEmpty(keyName) || String.IsNullOrWhiteSpace(keyName))
            {
                throw new ArgumentNullException("keyName");
            }

            if (varyParams != null && varyParams.Count > 0)
            {
                m_cacheVaryParams = varyParams;
            }
        }

        /// <summary>
        /// Create a new instance of CacheKey
        /// </summary>
        /// <param name="keyName">The cache key name</param>
        /// <param name="varyParams">The vary params of this cache item</param>
        /// <example>
        /// CacheKey key = new CacheKey("keyName",new {PropA="value",PropB=2});
        /// </example>
        public CacheKey(string keyName, object varyParams)
        {
            //check preconditions for this input params!
            if (String.IsNullOrEmpty(keyName) || String.IsNullOrWhiteSpace(keyName))
            {
                throw new ArgumentNullException("keyName");
            }

            m_keyName = keyName;

            //extract vary params from this anonimous type
            ExtractVaryParams(varyParams);

        }


        #endregion

        #region -- Private Methods --

        private void ExtractVaryParams(object varyParams)
        {
            if (varyParams != null)
            {
                Type anonimousType = varyParams.GetType();
                var result = anonimousType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .Select(pi => new { Name = pi.Name, Value = pi.GetValue(varyParams, null) });

                if (result != null && result.Any())
                {
                    m_cacheVaryParams = new Dictionary<string, string>();
                    result.ToList().ForEach(item => m_cacheVaryParams.Add(item.Name, item.Value.ToString()));
                }
            }
        }

        #endregion

        #region -- Public Methods --

        /// <summary>
        /// Get composed cache key
        /// </summary>
        /// <returns>String represent the key for cache item</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetCacheKey()
        {
            StringBuilder keyBuilder = new StringBuilder();
            keyBuilder.Append(m_keyName);
            keyBuilder.Append("#");

            if (m_cacheVaryParams != null && m_cacheVaryParams.Count > 0)
            {
                foreach (var item in m_cacheVaryParams)
                {
                    keyBuilder.Append(string.Format(CultureInfo.InvariantCulture, "{0};{1}", item.Key, item.Value));
                    keyBuilder.Append(";");
                }
                //remove last ;
                --keyBuilder.Length;
            }

            return keyBuilder.ToString();
        }
        /// <summary>
        /// Override ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetCacheKey();
        }
        #endregion
    }
}
