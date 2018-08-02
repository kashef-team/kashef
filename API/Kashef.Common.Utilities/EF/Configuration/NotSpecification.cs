using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Kashef.Common.Utilities.EF.Configuraion
{
    /// <summary>
    /// NotEspecification convert a original specification with NOT logic operator
    /// </summary>
    /// <typeparam name="TEntity">Type of element for this specificaiton</typeparam>
    [DataContract(IsReference = true)]
#if !SILVERLIGHT
    [Serializable]
#endif 
    public class NotSpecification<TEntity> : Specification<TEntity> where TEntity : class,new()
    {
        #region -- Local Varaibles --

        [NonSerialized]
        private Expression<Func<TEntity, bool>> m_OriginalCriteria;

        #endregion

        #region -- Constructor --
        /// <summary>
        /// Constructor for NotSpecificaiton
        /// </summary>
        /// <param name="originalSpecification">Original specification</param>
        public NotSpecification(ISpecification<TEntity> originalSpecification)
        {

            if (originalSpecification == (ISpecification<TEntity>)null)
                throw new ArgumentNullException("originalSpecification");

            m_OriginalCriteria = originalSpecification.SatisfiedBy();
        }
        /// <summary>
        /// Constructor for NotSpecification
        /// </summary>
        /// <param name="originalSpecification">Original specificaiton</param>
        public NotSpecification(Expression<Func<TEntity, bool>> originalSpecification)
        {
            if (originalSpecification == (Expression<Func<TEntity, bool>>)null)
                throw new ArgumentNullException("originalSpecification");

            m_OriginalCriteria = originalSpecification;
        }
        #endregion

        #region -- Override Specification methods --

         public override Expression<Func<TEntity, bool>> SatisfiedBy()
        {

            return Expression.Lambda<Func<TEntity, bool>>(Expression.Not(m_OriginalCriteria.Body),
                                                         m_OriginalCriteria.Parameters.Single());
        }

        #endregion
    }
}
