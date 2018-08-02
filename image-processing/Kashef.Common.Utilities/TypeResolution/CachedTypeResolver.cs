using System;
using System.Collections;
using System.Collections.Specialized;
using Kashef.Common.Utilities.General.Strings;

namespace Kashef.Common.Utilities.TypeResolution
{   
    /// <summary>
    /// Resolves (instantiates) a <see cref="System.Type"/> by it's (possibly
    /// assembly qualified) name, and caches the <see cref="System.Type"/>
    /// instance against the type name.
    /// </summary>
    /// <author>Ahmed Al Amir</author>   
    public class CachedTypeResolver : ITypeResolver
    {
        #region -- Local Variables --
        /// <summary>
        /// The cache, mapping type names (<see cref="System.String"/> instances) against
        /// <see cref="System.Type"/> instances.
        /// </summary>
        private IDictionary m_typeCache = new HybridDictionary();

        private ITypeResolver m_typeResolver;

        #endregion

        #region -- Constructor (s) / Destructor  --

        /// <summary>
        /// Creates a new instance of the <see cref="Kashef.Common.Utilities.TypeResolution.CachedTypeResolver"/> class.
        /// </summary>
        /// <param name="typeResolver">
        /// The <see cref="Kashef.Common.Utilities.TypeResolution.ITypeResolver"/> that this instance will delegate
        /// actual <see cref="System.Type"/> resolution to if a <see cref="System.Type"/>
        /// cannot be found in this instance's <see cref="System.Type"/> cache.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="typeResolver"/> is <see langword="null"/>.
        /// </exception>
        public CachedTypeResolver(ITypeResolver typeResolver)
        {
            this.m_typeResolver = typeResolver;
        }

        /// <summary>
        /// Resolves the supplied <paramref name="typeName"/> to a
        /// <see cref="System.Type"/>
        /// instance.
        /// </summary>
        /// <param name="typeName">
        /// The (possibly partially assembly qualified) name of a
        /// <see cref="System.Type"/>.
        /// </param>
        /// <returns>
        /// A resolved <see cref="System.Type"/> instance.
        /// </returns>
        /// <exception cref="System.TypeLoadException">
        /// If the supplied <paramref name="typeName"/> could not be resolved
        /// to a <see cref="System.Type"/>.
        /// </exception>
        public Type Resolve(string typeName)
        {
            if (typeName.IsNullOrEmptyString())
            {
                throw BuildTypeLoadException(typeName);
            }
            Type type = null;
            try
            {
                lock (this.m_typeCache.SyncRoot)
                {
                    type = this.m_typeCache[typeName] as Type;
                    if (type == null)
                    {
                        type = this.m_typeResolver.Resolve(typeName);
                        this.m_typeCache[typeName] = type;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is TypeLoadException)
                {
                    throw;
                }
                throw BuildTypeLoadException(typeName, ex);
            }
            return type;
        }

        private static TypeLoadException BuildTypeLoadException(string typeName)
        {
            return new TypeLoadException("Could not load type from string value '" + typeName + "'.");
        }

        private static TypeLoadException BuildTypeLoadException(string typeName, Exception ex)
        {
            return new TypeLoadException("Could not load type from string value '" + typeName + "'.", ex);
        }

        #endregion
    }
   

}
