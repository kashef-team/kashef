using System.Data.Entity;

namespace Kashef.Common.Utilities.EF.Context
{
    /// <summary>
    /// This is the minimun contract for all context, one context per module, that extend
    /// base IContext contract with specific features of ADO .NET EF. 
    /// Creation of this and base contract add isolation feature from specific contract for
    /// testing purposed and delete innecesary dependencies.
    /// </summary>
    public interface IQueryableContext : IContext
    {
        /// <summary>
        /// Create a object set for a type TEntity
        /// </summary>
        /// <typeparam name="TEntity">Type of elements in object set</typeparam>
        /// <returns>Object set of type {TEntity}</returns>
        DbSet<TEntity> CreateObjectSet<TEntity>() where TEntity : class, new();

    }
}
