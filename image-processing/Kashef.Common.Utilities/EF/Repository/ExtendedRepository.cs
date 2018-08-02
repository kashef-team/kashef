using System;
using System.Collections.Generic;
using System.Data.Entity; 
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Kashef.Common.Utilities.Diagnostics.Trace; 
using Kashef.Common.Utilities.Properties;
using Kashef.Common.Utilities.EF.UnitOfWork;
using Kashef.Common.Utilities.EF.Configuraion;

namespace Kashef.Common.Utilities.EF.Repository
{
    /// <summary>
    /// Default base class for extended repostories. This generic repository 
    /// is a default implementation of <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/>
    /// and your specific repositories can inherit from this base class so automatically will get default implementation.
    /// IMPORTANT: Using this Base Repository class IS NOT mandatory. It is just a useful base class:
    /// You could also decide that you do not want to use this base Repository class, because sometimes you don't want a 
    /// specific Repository getting all these features and it might be wrong for a specific Repository. 
    /// For instance, you could want just read-only data methods for your Repository, etc. 
    /// in that case, just simply do not use this base class on your Repository.
    /// </summary>
    /// <typeparam name="TEntity">Type of elements in repostory</typeparam>
    public class ExtendedRepository<TEntity>
        : Repository<TEntity>, IExtendedRepository<TEntity>
        where TEntity : class
    {
        #region -- Local Variables --

        private IQueryableUnitOfWork m_CurrentUoW;


        #endregion

        #region -- Constructor --

        /// <summary>
        /// Default constructor for GenericRepository
        /// </summary>
        /// <param name="unitOfWork">A unit of work  for this repository</param>
        /// <param name="traceManager">TraceManager dependency</param>
        public ExtendedRepository(IQueryableUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            //check preconditions
            if (unitOfWork == (IQueryableUnitOfWork)null)
                throw new ArgumentNullException("unitOfWork", Resources.Exception_ContainerCannotBeNull);

            //set internal values
            m_CurrentUoW = unitOfWork;
        }

        #endregion

        #region -- IExtendedRepository<TEntity> --
        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/>
        /// </summary>
        /// <param name="items"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></param>
        public virtual void Add(params TEntity[] items)
        {
            //check arguments
            if (items == (TEntity[])null)
                throw new ArgumentNullException("items", Resources.Exception_ItemArgumentIsNull);

            //for each element in collection apply changes
            foreach (TEntity item in items)
            {
                base.Add(item);
            }
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/>
        /// </summary>
        /// <param name="items"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></param>
        public virtual void Modify(params TEntity[] items)
        {
            //check arguments
            if (items == (TEntity[])null)
                throw new ArgumentNullException("items", Resources.Exception_ItemArgumentIsNull);

            //for each element in collection apply changes
            foreach (TEntity item in items)
            {
                base.Modify(item);
            }
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/>
        /// </summary>
        /// <param name="items"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></param>
        public virtual void Remove(params TEntity[] items)
        {
            //check arguments
            if (items == (TEntity[])null)
                throw new ArgumentNullException("items", Resources.Exception_ItemArgumentIsNull);
            //for each element in collection apply changes
            foreach (TEntity item in items)
            {
                base.Remove(item);
            }
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/>
        /// </summary>
        /// <typeparam name="KEntity"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></typeparam>
        /// <param name="specification"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></param>
        /// <returns><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></returns>
        public virtual IEnumerable<KEntity> GetBySpec<KEntity>(ISpecification<KEntity> specification) where KEntity : class,TEntity
        {
            if (specification == (ISpecification<KEntity>)null)
                throw new ArgumentNullException("specification");

            
            return (m_CurrentUoW.CreateSet<TEntity>()
                            .OfType<KEntity>()
                            .Where(specification.SatisfiedBy())
                            .AsEnumerable<KEntity>());
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/>
        /// </summary>
        /// <typeparam name="K"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></typeparam>
        /// <returns><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></returns>
        public virtual IEnumerable<K> GetAll<K>() where K : TEntity
        {
             //Create IObjectSet and perform query
            return (m_CurrentUoW.CreateSet<TEntity>().OfType<K>()).AsEnumerable<K>();
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/>
        /// </summary>
        /// <typeparam name="K"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></typeparam>
        /// <typeparam name="S"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></typeparam>
        /// <param name="pageIndex"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></param>
        /// <param name="pageCount"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></param>
        /// <param name="orderByExpression"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></param>
        /// <param name="ascending"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></param>
        /// <returns><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></returns>
        public virtual IEnumerable<K> GetPagedElements<K, S>(int pageIndex, int pageCount, System.Linq.Expressions.Expression<Func<K, S>> orderByExpression, bool ascending)
            where K : TEntity
        {
            //checking query arguments
            if (pageIndex < 0)
                throw new ArgumentException(Resources.Exception_InvalidPageIndex, "pageIndex");

            if (pageCount <= 0)
                throw new ArgumentException(Resources.Exception_InvalidPageCount, "pageCount");

            if (orderByExpression == (Expression<Func<K, S>>)null)
                throw new ArgumentNullException("orderByExpression", Resources.Exception_OrderByExpressionCannotBeNull);

            
            //Create IObjectSet for this type and perform query
            DbSet<TEntity> objectSet = m_CurrentUoW.CreateSet<TEntity>();

            return (ascending)
                                ?
                                    objectSet.OfType<K>()
                                     .OrderBy(orderByExpression)
                                     .Skip(pageIndex * pageCount)
                                     .Take(pageCount)
                                     .ToList()
                                :
                                    objectSet.OfType<K>()
                                     .OrderByDescending(orderByExpression)
                                     .Skip(pageIndex * pageCount)
                                     .Take(pageCount)
                                     .ToList();
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/>
        /// </summary>
        /// <typeparam name="K"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></typeparam>
        /// <param name="filter"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></param>
        /// <returns><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></returns>
        public virtual IEnumerable<K> GetFilteredElements<K>(Expression<Func<K, bool>> filter)
            where K : TEntity
        {
            //checking query arguments
            if (filter == (Expression<Func<K, bool>>)null)
                throw new ArgumentNullException("filter", Resources.Exception_FilterCannotBeNull);

           //Create IObjectSet and perform query
            return m_CurrentUoW.CreateSet<TEntity>()
                             .OfType<K>()
                             .Where(filter)
                             .ToList();
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/>
        /// </summary>
        /// <typeparam name="K"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></typeparam>
        /// <typeparam name="S"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></typeparam>
        /// <param name="filter"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></param>
        /// <param name="orderByExpression"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></param>
        /// <param name="ascending"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></param>
        /// <returns></returns>
        public virtual IEnumerable<K> GetFilteredElements<K, S>(Expression<Func<K, bool>> filter, Expression<Func<K, S>> orderByExpression, bool ascending)
            where K : TEntity
        {
            //checking query arguments
            if (filter == (Expression<Func<K, bool>>)null)
                throw new ArgumentNullException("filter", Resources.Exception_FilterCannotBeNull);

            if (orderByExpression == (Expression<Func<K, S>>)null)
                throw new ArgumentNullException("orderByExpression", Resources.Exception_OrderByExpressionCannotBeNull);

      
            //Create IObjectSet for this particular type and query this

            DbSet<TEntity> objectSet = m_CurrentUoW.CreateSet<TEntity>();

            return (ascending)
                                ?
                                    objectSet.OfType<K>()
                                     .Where(filter)
                                     .OrderBy(orderByExpression)
                                     .ToList()
                                :
                                    objectSet.OfType<K>()
                                     .Where(filter)
                                     .OrderByDescending(orderByExpression)
                                     .ToList();
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/>
        /// </summary>
        /// <typeparam name="K"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></typeparam>
        /// <param name="filter"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></param>
        /// <param name="orderByExpression"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></param>
        /// <param name="ascending"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IExtendedRepository{TEntity}"/></param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetFilteredElements<K>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, K>> orderByExpression, bool ascending)
        {
            //checking query arguments
            if (filter == (Expression<Func<TEntity, bool>>)null)
                throw new ArgumentNullException("filter", Resources.Exception_FilterCannotBeNull);

            if (orderByExpression == (Expression<Func<TEntity, K>>)null)
                throw new ArgumentNullException("orderByExpression", Resources.Exception_OrderByExpressionCannotBeNull);

         
            //Create IObjectSet for this particular type and query this

            DbSet<TEntity> objectSet = m_CurrentUoW.CreateSet<TEntity>();

            return (ascending)
                                ?
                                    objectSet.OfType<TEntity>()
                                     .Where(filter)
                                     .OrderBy(orderByExpression)
                                     .ToList()
                                :
                                    objectSet.OfType<TEntity>()
                                     .Where(filter)
                                     .OrderByDescending(orderByExpression)
                                     .ToList();
        }
        #endregion
    }
}
