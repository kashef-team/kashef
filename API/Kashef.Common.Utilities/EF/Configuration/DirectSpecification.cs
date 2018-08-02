using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Kashef.Common.Utilities.EF.Configuraion
{
    /// <summary>
    /// A Direct Specification is a simple implementation
    /// of specification that acquire this from a lambda expression
    /// in  constructor
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that check this specification</typeparam>
    [DataContract(IsReference = true)]
#if !SILVERLIGHT
    [Serializable]
#endif 
    public class DirectSpecification<TEntity> : Specification<TEntity> where TEntity : class,new()
    {
        #region -- Local Variables --
        [NonSerialized]
        private Expression<Func<TEntity, bool>> m_MatchingCriteria;
        #endregion

        #region -- Constructor--

        /// <summary>
        /// Default constructor for Direct Specification
        /// </summary>
        /// <param name="matchingCriteria">A Matching Criteria</param>
        public DirectSpecification(Expression<Func<TEntity, bool>> matchingCriteria)
        {
            if (matchingCriteria == (Expression<Func<TEntity, bool>>)null)
                throw new ArgumentNullException("matchingCriteria");

            m_MatchingCriteria = matchingCriteria;
        }

        #endregion

        #region -- Override --

       
        public override Expression<Func<TEntity, bool>> SatisfiedBy()
        {
            return m_MatchingCriteria;
        }

        #endregion
    }
}
