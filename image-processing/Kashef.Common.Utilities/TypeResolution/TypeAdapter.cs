using System;
using System.Collections.Generic;
using Kashef.Common.Utilities.Properties;

namespace Kashef.Common.Utilities.TypeResolution
{    
    /// <summary>
    /// ITypeAdatper implementation. 
    /// <remarks>
    /// Really, this implementation only load RegisterTypesMapConfiguration 
    /// and create a global dictionary of mappers.
    /// </remarks.
    /// </summary>
    public sealed class TypeAdapter
        : ITypeAdapter
    {
        #region -- Local Varaibles --
        private Dictionary<string, ITypeMapConfigurationBase> m_maps;
        #endregion

        #region -- Properties --
        /// <summary>
        /// Get the collection of ITypeMapConfigurationBase elements
        /// </summary>
        public Dictionary<string, ITypeMapConfigurationBase> Maps
        {
            get
            {
                return m_maps;
            }
        }

        #endregion

        #region --  Constructor --
        /// <summary>
        /// Create a instance of TypeAdapter
        /// </summary>
        public TypeAdapter(RegisterTypesMap[] mapModules)
        {
            InitializeAdapter(mapModules);
        }

        #endregion

        #region -- Private Method --

        private void InitializeAdapter(RegisterTypesMap[] mapsModules)
        {
            //create map adapters dictionary
            m_maps = new Dictionary<string, ITypeMapConfigurationBase>();

            if (mapsModules != null)
            {
                //foreach adapter's module in solution load mapping
                foreach (var module in mapsModules)
                {
                    foreach (var map in module.Maps)
                        m_maps.Add(map.Key, map.Value);
                }
            }
        }

        #endregion

        #region -- ITypeAdapter Implementation --

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.TypeResolution.ITypeAdapter"/>
        /// </summary>
        /// <typeparam name="TSource"><see cref="Kashef.Common.Utilities.TypeResolution.ITypeAdapter"/></typeparam>
        /// <typeparam name="TTarget"><see cref="Kashef.Common.Utilities.TypeResolution.ITypeAdapter"/></typeparam>
        /// <param name="source"><see cref="Kashef.Common.Utilities.TypeResolution.ITypeAdapter"/></param>
        /// <returns><see cref="Kashef.Common.Utilities.TypeResolution.ITypeAdapter"/></returns>
        public TTarget Adapt<TSource, TTarget>(TSource source)
            where TSource : class
            where TTarget : class, new()
        {
            if (source == (TSource)null)
                throw new ArgumentNullException("source");

            var descriptor = TypeMapConfigurationBase<TSource, TTarget>.GetDescriptor();

            if (m_maps.ContainsKey(descriptor))
            {
                var spec = m_maps[descriptor] as TypeMapConfigurationBase<TSource, TTarget>;

                return spec.Resolve(source);
            }
            else
                throw new InvalidOperationException(string.Format(Resources.Exception_NotMapFoundForTypeAdapter,
                                                                typeof(TSource).FullName,
                                                                typeof(TTarget).FullName));
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.TypeResolution.ITypeAdapter"/>
        /// </summary>
        /// <typeparam name="TSource"><see cref="Kashef.Common.Utilities.TypeResolution.ITypeAdapter"/></typeparam>
        /// <typeparam name="TTarget"><see cref="Kashef.Common.Utilities.TypeResolution.ITypeAdapter"/></typeparam>
        /// <param name="source"><see cref="Kashef.Common.Utilities.TypeResolution.ITypeAdapter"/></param>
        /// <param name="moreSources"><see cref="Kashef.Common.Utilities.TypeResolution.ITypeAdapter"/></param>
        /// <returns><see cref="Kashef.Common.Utilities.TypeResolution.ITypeAdapter"/></returns>
        public TTarget Adapt<TSource, TTarget>(TSource source, params object[] moreSources)
            where TSource : class
            where TTarget : class, new()
        {

            if (source == (TSource)null)
                throw new ArgumentNullException("source");

            string descriptor = TypeMapConfigurationBase<TSource, TTarget>.GetDescriptor();

            if (m_maps.ContainsKey(descriptor))
            {
                var spec = m_maps[descriptor] as TypeMapConfigurationBase<TSource, TTarget>;

                return spec.Resolve(source, moreSources);
            }
            else
                throw new InvalidOperationException(string.Format(Resources.Exception_NotMapFoundForTypeAdapter,
                                                                typeof(TSource).FullName,
                                                                typeof(TTarget).FullName));
        }

        #endregion
    }
}
