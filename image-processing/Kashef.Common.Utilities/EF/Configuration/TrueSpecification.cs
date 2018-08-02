using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Kashef.Common.Utilities.EF.Configuraion
{
    /// <summary>
    /// True specification.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity in this specification</typeparam>
    [DataContract(IsReference = true)]
#if !SILVERLIGHT
    [Serializable]
#endif 
    public class TrueSpecification<TEntity>
        : Specification<TEntity>
        where TEntity : class,new()
    {
        #region -- Specification overrides --

         public override System.Linq.Expressions.Expression<Func<TEntity, bool>> SatisfiedBy()
        {
            Expression<Func<TEntity, bool>> trueExpression = t => true;
            return trueExpression;
        }

        #endregion
    }
}
