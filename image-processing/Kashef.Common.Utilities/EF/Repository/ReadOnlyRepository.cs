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
    /// Default base class for repostories. This generic repository 
    /// is a default implementation of <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IReadOnlyRepository{TEntity}"/>
    /// and your specific repositories can inherit from this base class so automatically will get default implementation.
    /// IMPORTANT: Using this Base Repository class IS NOT mandatory. It is just a useful base class:
    /// You could also decide that you do not want to use this base Repository class, because sometimes you don't want a 
    /// specific Repository getting all these features and it might be wrong for a specific Repository. 
    /// For instance, you could want just read-only data methods for your Repository, etc. 
    /// in that case, just simply do not use this base class on your Repository.
    /// </summary>
    /// <typeparam name="TEntity">Type of elements in repostory</typeparam>
    public class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity>
        where TEntity : class
    {
        #region -- Local Variales --
       
        /// <summary>
        /// Reference to unit of work.
        /// </summary>
        private IQueryableUnitOfWork m_UnitOfWork;
        #endregion

        #region -- Constructor --
        /// <summary>
        /// Create a new instance of repository
        /// </summary>
        /// <param name="unitOfWork">Associated Unit Of Work</param>
        /// <param name="traceManager">Trace store.</param>
        public ReadOnlyRepository(IQueryableUnitOfWork unitOfWork)
        {
            //check preconditions
            if (unitOfWork == (IQueryableUnitOfWork)null)
                throw new ArgumentNullException("unitOfWork", Resources.Exception_ContainerCannotBeNull);
             
            //set internal values
            m_UnitOfWork = unitOfWork;

            ((DbContext)this.UnitOfWork).Database.Log = Console.Write;
        }

        #endregion

        #region -- Private Methods --
        /// <summary>
        /// Gets IObjectSet from the unit of work.
        /// </summary>
        /// <returns></returns>
        protected DbSet<TEntity> GetSet()
        {
            return this.m_UnitOfWork.CreateSet<TEntity>();
        }
        #endregion

        #region -- IReadOnlyRepository Methods --
       
        /// <summary>
        /// Return a unit of work in this repository
        /// </summary>
        public virtual IUnitOfWork UnitOfWork
        {
            get
            {
                return this.m_UnitOfWork;
            }
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>
        /// <returns><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></returns>
        public virtual IEnumerable<TEntity> GetAll()
        { 
            return GetSet().AsEnumerable<TEntity>();
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>
        /// <returns><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></returns>
        public virtual IEnumerable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            if (includeProperties == (Expression<Func<TEntity, object>>[])null)
                throw new ArgumentNullException("includeProperties");

          
            IQueryable<TEntity> result = this.GetSet() as IQueryable<TEntity>;
            if (result != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    result = result.Include(includeProperty);
                }
            }
            return result;
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="specification"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <returns><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></returns>
        public virtual IEnumerable<TEntity> GetBySpec(ISpecification<TEntity> specification)
        {
            if (specification == (ISpecification<TEntity>)null)
                throw new ArgumentNullException("specification");

            return GetSet().Where(specification.SatisfiedBy());
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="specification"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="includeProperties"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <returns><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></returns>
        public virtual IEnumerable<TEntity> GetBySpec(ISpecification<TEntity> specification, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            if (specification == (ISpecification<TEntity>)null)
                throw new ArgumentNullException("specification");

            if (includeProperties == (Expression<Func<TEntity, object>>[])null)
                throw new ArgumentNullException("includeProperties");

           
            IQueryable<TEntity> result = this.GetSet() as IQueryable<TEntity>;
            if (result != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    result = result.Include(includeProperty);
                }
            }
            return result.Where(specification.SatisfiedBy());
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="pageIndex"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="pageCount"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="orderByExpression"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="ascending"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <returns><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></returns>
        public virtual IEnumerable<TEntity> GetPagedElements<S>(int pageIndex, int pageCount, Expression<Func<TEntity, S>> orderByExpression, bool ascending)
        {
            if (pageIndex < 0)
                throw new ArgumentException(Resources.Exception_InvalidPageIndex, "pageIndex");

            if (pageCount <= 0)
                throw new ArgumentException(Resources.Exception_InvalidPageCount, "pageCount");

            if (orderByExpression == (Expression<Func<TEntity, S>>)null)
                throw new ArgumentNullException("orderByExpression", Resources.Exception_OrderByExpressionCannotBeNull);

            var set = GetSet();

            if (ascending)
            {
                return set.OrderBy(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
            else
            {
                return set.OrderByDescending(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="pageIndex"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="pageCount"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="orderByExpression"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="ascending"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="includeProperties"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <returns><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></returns>
        public virtual IEnumerable<TEntity> GetPagedElements<S>(int pageIndex, int pageCount, Expression<Func<TEntity, S>> orderByExpression, bool ascending, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            if (pageIndex < 0)
                throw new ArgumentException(Resources.Exception_InvalidPageIndex, "pageIndex");

            if (pageCount <= 0)
                throw new ArgumentException(Resources.Exception_InvalidPageCount, "pageCount");

            if (orderByExpression == (Expression<Func<TEntity, S>>)null)
                throw new ArgumentNullException("orderByExpression", Resources.Exception_OrderByExpressionCannotBeNull);

            if (includeProperties == (Expression<Func<TEntity, object>>[])null)
                throw new ArgumentNullException("includeProperties");

          
            IQueryable<TEntity> set = this.GetSet() as IQueryable<TEntity>;
            if (set != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    set = set.Include(includeProperty);
                }
            }

            if (ascending)
            {
                return set.OrderBy(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
            else
            {
                return set.OrderByDescending(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="filter"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <returns><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></returns>
        public virtual IEnumerable<TEntity> GetFilteredElements(Expression<Func<TEntity, bool>> filter)
        {
            if (filter == (Expression<Func<TEntity, bool>>)null)
                throw new ArgumentNullException("filter", Resources.Exception_FilterCannotBeNull);
            
            return GetSet().Where(filter);
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="filter"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="includeProperties"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <returns><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></returns>
        public virtual IEnumerable<TEntity> GetFilteredElements(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            if (filter == (Expression<Func<TEntity, bool>>)null)
                throw new ArgumentNullException("filter", Resources.Exception_FilterCannotBeNull);

            if (includeProperties == (Expression<Func<TEntity, object>>[])null)
                throw new ArgumentNullException("includeProperties");

            
            IQueryable<TEntity> set = this.GetSet() as IQueryable<TEntity>;
            if (set != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    set = set.Include(includeProperty);
                }
            }
            return set.Where(filter);
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="filter"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="pageIndex"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="pageCount"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="orderByExpression"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="ascending"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <returns><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></returns>
        public virtual IEnumerable<TEntity> GetFilteredElements<S>(Expression<Func<TEntity, bool>> filter, int pageIndex, int pageCount, Expression<Func<TEntity, S>> orderByExpression, bool ascending)
        {
            if (filter == (Expression<Func<TEntity, bool>>)null)
                throw new ArgumentNullException("filter", Resources.Exception_FilterCannotBeNull);

            if (pageIndex < 0)
                throw new ArgumentException(Resources.Exception_InvalidPageIndex, "pageIndex");

            if (pageCount <= 0)
                throw new ArgumentException(Resources.Exception_InvalidPageCount, "pageCount");

            if (orderByExpression == (Expression<Func<TEntity, S>>)null)
                throw new ArgumentNullException("orderByExpression", Resources.Exception_OrderByExpressionCannotBeNull);

           var set = GetSet();

            if (ascending)
            {
                return set.Where(filter).OrderBy(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
            else
            {
                return set.Where(filter).OrderByDescending(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="filter"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="pageIndex"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="pageCount"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="orderByExpression"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="ascending"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="includeProperties"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <returns><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></returns>
        public virtual IEnumerable<TEntity> GetFilteredElements<S>(System.Linq.Expressions.Expression<Func<TEntity, bool>> filter, int pageIndex, int pageCount, Expression<Func<TEntity, S>> orderByExpression, bool ascending, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            if (filter == (Expression<Func<TEntity, bool>>)null)
                throw new ArgumentNullException("filter", Resources.Exception_FilterCannotBeNull);

            if (pageIndex < 0)
                throw new ArgumentException(Resources.Exception_InvalidPageIndex, "pageIndex");

            if (pageCount <= 0)
                throw new ArgumentException(Resources.Exception_InvalidPageCount, "pageCount");

            if (orderByExpression == (Expression<Func<TEntity, S>>)null)
                throw new ArgumentNullException("orderByExpression", Resources.Exception_OrderByExpressionCannotBeNull);

            if (includeProperties == (Expression<Func<TEntity, object>>[])null)
                throw new ArgumentNullException("includeProperties");

            
            IQueryable<TEntity> set = this.GetSet() as IQueryable<TEntity>;
            if (set != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    set = set.Include(includeProperty);
                }
            }

            if (ascending)
            {
                return set.Where(filter).OrderBy(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
            else
            {
                return set.Where(filter).OrderByDescending(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>
        /// <typeparam name="S"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></typeparam>
        /// <param name="pageIndex"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="pageCount"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="orderByExpression"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="specification"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="ascending"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <returns><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></returns>        
        public virtual IEnumerable<TEntity> GetPagedElements<S>(int pageIndex, int pageCount, Expression<Func<TEntity, S>> orderByExpression, ISpecification<TEntity> specification, bool ascending)
        {
            if (specification == (ISpecification<TEntity>)null)
                throw new ArgumentNullException("specification", Resources.Exception_FilterCannotBeNull);

            if (pageIndex < 0)
                throw new ArgumentException(Resources.Exception_InvalidPageIndex, "pageIndex");

            if (pageCount <= 0)
                throw new ArgumentException(Resources.Exception_InvalidPageCount, "pageCount");

            if (orderByExpression == (Expression<Func<TEntity, S>>)null)
                throw new ArgumentNullException("orderByExpression", Resources.Exception_OrderByExpressionCannotBeNull);

            var set = GetSet();

            if (ascending)
            {
                return set.Where(specification.SatisfiedBy()).OrderBy(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
            else
            {
                return set.Where(specification.SatisfiedBy()).OrderByDescending(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>
        /// <typeparam name="S"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></typeparam>
        /// <param name="pageIndex"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="pageCount"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="orderByExpression"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="specification"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="ascending"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <param name="includeProperties"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        /// <returns><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></returns>        
        public virtual IEnumerable<TEntity> GetPagedElements<S>(int pageIndex, int pageCount, Expression<Func<TEntity, S>> orderByExpression, ISpecification<TEntity> specification, bool ascending, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            if (specification == (ISpecification<TEntity>)null)
                throw new ArgumentNullException("specification", Resources.Exception_FilterCannotBeNull);

            if (pageIndex < 0)
                throw new ArgumentException(Resources.Exception_InvalidPageIndex, "pageIndex");

            if (pageCount <= 0)
                throw new ArgumentException(Resources.Exception_InvalidPageCount, "pageCount");

            if (orderByExpression == (Expression<Func<TEntity, S>>)null)
                throw new ArgumentNullException("orderByExpression", Resources.Exception_OrderByExpressionCannotBeNull);

            if (includeProperties == (Expression<Func<TEntity, object>>[])null)
                throw new ArgumentNullException("includeProperties");

         
            IQueryable<TEntity> set = this.GetSet() as IQueryable<TEntity>;
            if (set != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    set = set.Include(includeProperty);
                }
            }

            if (ascending)
            {
                return set.Where(specification.SatisfiedBy()).OrderBy(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
            else
            {
                return set.Where(specification.SatisfiedBy()).OrderByDescending(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>              
        public virtual void Dispose()
        {
            if (this.m_UnitOfWork != null)
                this.m_UnitOfWork.Dispose();
        }
        #endregion

        #region Scalar operations

        public virtual long LongCount()
        { 
            return this.GetSet().LongCount<TEntity>();
        }

        public virtual long LongCount(Expression<Func<TEntity, bool>> filter)
        {
            return this.GetSet().LongCount<TEntity>(filter);
        } 

        public virtual int Max(Func<TEntity, int> perdicate) 
        {
            return this.GetSet().Max<TEntity>(perdicate);
        } 

        #endregion
    }
}
