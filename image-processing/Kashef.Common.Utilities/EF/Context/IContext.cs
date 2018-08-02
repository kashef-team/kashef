using Kashef.Common.Utilities.EF.UnitOfWork;
using System;

namespace Kashef.Common.Utilities.EF.Context
{
    /// <summary>
    /// Really Context is the main class for interact with data and this is
    /// used usually in repositories for storing and query data. For mantein PI
    /// into Domain Layer and add features for Mocking this context is expressed 
    /// as interfaces and fluent this into repositories implementation.
    /// </summary>
    public interface IContext : IUnitOfWork, IDisposable
    {
        /// <summary>
        /// Apply changes made in item or related items in your graph
        /// </summary>
        /// <typeparam name="TEntity">Type of item</typeparam>
        /// <param name="item">Item with changes</param>
        void SetChanges<TEntity>(TEntity item) where TEntity : class, new();
    }
}
