
using System.Collections.Generic;
namespace Kashef.Common.Utilities.EF.Repository
{
    /// <summary>
    /// Base interface for implement a "Repository Pattern", for
    /// more information about this pattern see http://martinfowler.com/eaaCatalog/repository.html
    /// or http://blogs.msdn.com/adonet/archive/2009/06/16/using-repository-and-unit-of-work-patterns-with-entity-framework-4-0.aspx
    /// </summary>
    /// <remarks>
    /// Indeed, one might think that IObjectSet is already a generic repository and therefore
    /// would not need this item. Using this interface allows us to ensure PI principle
    /// within our domain model.
    /// </remarks>
    /// <typeparam name="TEntity">Type of entity for this repository </typeparam>
    public interface IRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Add item into repository
        /// </summary>
        /// <param name="item">Item to add to repository</param>
        void Add(TEntity item);

        /// <summary>
        /// Add set of items into repository
        /// </summary>
        /// <param name="items"></param>
        void AddRange(IEnumerable<TEntity> items);

        /// <summary>
        ///  Add items into repository
        /// </summary>
        /// <param name="items"></param>
       // void Add(System.Collections.Generic.List<TEntity> items);

        /// <summary>
        /// Delete item 
        /// </summary>
        /// <param name="item">Item to delete</param>
        void Remove(TEntity item);

        /// <summary>
        /// Delete List of items from repository
        /// </summary>
        /// <param name="items"></param>
        void RemoveRange(IEnumerable<TEntity> items);

        /// <summary>
        /// Sets modified entity into the repository. 
        /// When calling Commit() method in UnitOfWork 
        /// these changes will be saved into the storage
        /// <remarks>
        /// Internally this method always calls Repository.Attach() and Context.SetChanges() 
        /// </remarks>
        /// </summary>
        /// <param name="item">Item with changes</param>
        void Modify(TEntity item);

        /// <summary>
        /// Sets modified entity into the repository. 
        /// When calling Commit() method in UnitOfWork 
        /// these changes will be saved into the storage
        /// <remarks>
        /// Internally this method always calls Repository.Attach() and Context.SetChanges() 
        /// </remarks>
        /// </summary>
        /// <param name="item">Item with changes</param>
        //void Modify(System.Collections.Generic.List<TEntity> items);
    }
}
