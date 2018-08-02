using System;
using System.Collections.Generic;

namespace Kashef.Common.Utilities.TypeResolution
{    
    /// <summary>
    /// Base class from ModuleTypesAdapter. This class
    /// is intended to subclasing for each module in 
    /// solution to configure specific maps
    /// </summary>
    public abstract class RegisterTypesMap
    {
        #region -- Local Variables --
        private Dictionary<string, ITypeMapConfigurationBase> m_maps;
        #endregion

        #region --  Properties --
        /// <summary>
        /// Get the dictionary of type maps
        /// </summary>
        public Dictionary<string, ITypeMapConfigurationBase> Maps
        {
            get
            {
                return m_maps;
            }
        }


        #endregion

        #region -- Constructor --
        /// <summary>
        /// Create a new instance of ModulesTypeAdapter
        /// </summary>
        public RegisterTypesMap()
        {
            //create a new instance of type maps dictionary
            m_maps = new Dictionary<string, ITypeMapConfigurationBase>();
        }

        #endregion

        #region -- Public Abstract Methods --

        /// <summary>
        /// Register map into this register types map
        /// </summary>
        /// <typeparam name="TSource">The source type</typeparam>
        /// <typeparam name="TTarget">The target type</typeparam>
        /// <param name="map">The map to register</param>
        public void RegisterMap<TSource, TTarget>(TypeMapConfigurationBase<TSource, TTarget> map)
            where TSource : class
            where TTarget : class,new()
        {
            if (map == (TypeMapConfigurationBase<TSource, TTarget>)null)
                throw new ArgumentNullException("map");

            m_maps.Add(map.Descriptor, map);
        }

        #endregion
    }
}
