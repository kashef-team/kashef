using System.Runtime.Serialization;
using System;
namespace Kashef.Common.Utilities.EF.Configuraion
{
    /// <summary>
    /// Base class for composite specifications
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that check this specification</typeparam>
    [DataContract(IsReference = true)]
#if !SILVERLIGHT
    [Serializable]
#endif 
    public abstract class CompositeSpecification<TEntity> : Specification<TEntity> where TEntity : class,new()
    {
        #region -- Properties --

        /// <summary>
        /// Left side specification for this composite element
        /// </summary>
        public abstract ISpecification<TEntity> LeftSideSpecification { get; }

        /// <summary>
        /// Right side specification for this composite element
        /// </summary>
        public abstract ISpecification<TEntity> RightSideSpecification { get; }

        #endregion
    }
}
