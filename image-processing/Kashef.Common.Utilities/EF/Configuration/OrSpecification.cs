using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Kashef.Common.Utilities.EF.Configuraion
{
    /// <summary>
    /// A Logic OR Specification
    /// </summary>
    /// <typeparam name="T">Type of entity that check this specification</typeparam>
    [DataContract(IsReference = true)]
#if !SILVERLIGHT
    [Serializable]
#endif 
    public class OrSpecification<T> : CompositeSpecification<T> where T : class, new()
    {
        #region -- Local Varaibles --
        private ISpecification<T> m_RightSideSpecification = null;
        private ISpecification<T> m_LeftSideSpecification = null;

        #endregion

        #region -- Constructor --
        /// <summary>
        /// Default constructor for AndSpecification
        /// </summary>
        /// <param name="leftSide">Left side specification</param>
        /// <param name="rightSide">Right side specification</param>
        public OrSpecification(ISpecification<T> leftSide, ISpecification<T> rightSide)
        {
            if (leftSide == (ISpecification<T>)null)
                throw new ArgumentNullException("leftSide");

            if (rightSide == (ISpecification<T>)null)
                throw new ArgumentNullException("rightSide");

            this.m_LeftSideSpecification = leftSide;
            this.m_RightSideSpecification = rightSide;
        }

        #endregion

        #region -- Composite Specification overrides --
        /// <summary>
        /// Left side specification
        /// </summary>
        public override ISpecification<T> LeftSideSpecification
        {
            get { return m_LeftSideSpecification; }
        }
        /// <summary>
        /// Righ side specification
        /// </summary>
        public override ISpecification<T> RightSideSpecification
        {
            get { return m_RightSideSpecification; }
        }
      
        public override Expression<Func<T, bool>> SatisfiedBy()
        {
            Expression<Func<T, bool>> left = m_LeftSideSpecification.SatisfiedBy();
            Expression<Func<T, bool>> right = m_RightSideSpecification.SatisfiedBy();

            return (left.Or(right));

        }
        #endregion
    }
}
