using System;

namespace Kashef.Common.Utilities.TypeResolution
{    
    /// <summary>
    /// Fluent Type map configuration
    /// <remarks>
    /// This is the base class intented to use in "agile mapping", for 
    /// example for mapping test 
    /// <example>
    /// var spec = new MapSpec{Customer,CustomerDTO}().Pre((s)=>{})
    ///                                               .Map((s)=>Mapper.Map(s))
    ///                                               .Post((t,items)=>{});
    /// </example>
    /// </remarks>
    /// </summary>
    /// <typeparam name="TSource">The source type</typeparam>
    /// <typeparam name="TTarget">The target type</typeparam>
    public sealed class TypeMapConfiguration<TSource, TTarget>
        : TypeMapConfigurationBase<TSource, TTarget>
        where TSource : class
        where TTarget : class,new()
    {
        #region -- Local Variables --
        private Action<TSource> m_beforeMapAction = null;
        private Func<TSource, TTarget> m_mapFunction = null;
        private Action<TTarget, object[]> m_afterMapAction = null;
        #endregion

        #region -- Public Methods --

        /// <summary>
        /// Configure before map action
        /// </summary>
        /// <param name="beforeMapAction">The pre action</param>
        /// <returns>This</returns>
        public TypeMapConfiguration<TSource, TTarget> Before(Action<TSource> beforeMapAction)
        {
            m_beforeMapAction = beforeMapAction;

            return this;
        }
        /// <summary>
        /// Configure map function
        /// </summary>
        /// <param name="mapFunction">The map function to use</param>
        /// <returns>This</returns>
        public TypeMapConfiguration<TSource, TTarget> Map(Func<TSource, TTarget> mapFunction)
        {
            m_mapFunction = mapFunction;

            return this;
        }
        /// <summary>
        /// Configure the after map action
        /// </summary>
        /// <param name="afterMapAction">The post function</param>
        /// <returns>This</returns>
        public TypeMapConfiguration<TSource, TTarget> After(Action<TTarget, object[]> afterMapAction)
        {
            m_afterMapAction = afterMapAction;

            return this;
        }

        #endregion

        #region -- TypeMapConfigurationBase Members --
        /// <summary>
        /// This method is called before mapping on source entity.
        /// </summary>
        /// <param name="source"></param>
        protected override void BeforeMap(ref TSource source)
        {
            //call to befor map action
            if (m_beforeMapAction != null)
                m_beforeMapAction(source);
        }

        /// <summary>
        /// This method is called after the mapping on target entity.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="moreSources"></param>
        protected override void AfterMap(ref TTarget target, params object[] moreSources)
        {
            //call to after map action
            if (m_afterMapAction != null)
                m_afterMapAction(target, moreSources);
        }

        /// <summary>
        /// Does the actual maping by calling mapping function.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>target object, throws exception if no map is specified.</returns>
        protected override TTarget Map(TSource source)
        {
            //call map function
            if (m_mapFunction != null)
                return m_mapFunction(source);
            else
                throw new InvalidOperationException("Map is not specified!");
        }

        #endregion
    }
   
}
