using System;
using System.Data.Entity; 
using System.Globalization;
using Kashef.Common.Utilities.Diagnostics.Trace; 
using Kashef.Common.Utilities.Properties;
using Kashef.Common.Utilities.EF.UnitOfWork;
using System.Collections.Generic;

namespace Kashef.Common.Utilities.EF.Repository
{
    /// <summary>
    /// Default base class for repostories. This generic repository 
    /// is a default implementation of <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
    /// and your specific repositories can inherit from this base class so automatically will get default implementation.
    /// IMPORTANT: Using this Base Repository class IS NOT mandatory. It is just a useful base class:
    /// You could also decide that you do not want to use this base Repository class, because sometimes you don't want a 
    /// specific Repository getting all these features and it might be wrong for a specific Repository. 
    /// For instance, you could want just read-only data methods for your Repository, etc. 
    /// in that case, just simply do not use this base class on your Repository.
    /// </summary>
    /// <typeparam name="TEntity">Type of elements in repository</typeparam>
    public class Repository<TEntity> : ReadOnlyRepository<TEntity>, IRepository<TEntity>
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
        /// Create a new instance of Repository
        /// </summary>
        /// <param name="traceManager">Trace Manager dependency</param>
        /// <param name="unitOfWork">A unit of work for this repository</param>
        public Repository(IQueryableUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            //check preconditions
            if (unitOfWork == (IQueryableUnitOfWork)null)
                throw new ArgumentNullException("unitOfWork", Resources.Exception_ContainerCannotBeNull);

            //set internal values
            m_UnitOfWork = unitOfWork;

        }

        #endregion

        #region -- IRepository<TEntity> Members --
        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="item"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        public virtual void Add(TEntity item)
        {
            //check item
            if (item == (TEntity)null)
                throw new ArgumentNullException("item", Resources.Exception_ItemArgumentIsNull);

            this.GetSet().Add(item);

        }

        public virtual void AddRange(IEnumerable<TEntity> items)
        {
            if (null == items)
            {
                throw new ArgumentNullException("items", Resources.Exception_ItemArgumentIsNull);
            }

            this.GetSet().AddRange(items);
        }
         
        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="item"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        public virtual void Remove(TEntity item)
        {
            //check item
            if (item == (TEntity)null)
                throw new ArgumentNullException("item", Resources.Exception_ItemArgumentIsNull);


            DbSet<TEntity> objectSet = GetSet();

            //Attach object to unit of work and delete this
            // this is valid only if T is a type in model
            objectSet.Attach(item);

            //delete object to IObjectSet for this type
            objectSet.Remove(item);
        }


        public void RemoveRange(IEnumerable<TEntity> items)
        {
            if (null == items)
            {
                throw new ArgumentNullException("items", Resources.Exception_ItemArgumentIsNull);
            } 
            this.GetSet().RemoveRange(items);
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="item"><see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/></param>
        public virtual void Modify(TEntity item)
        {
            //check arguments
            if (item == (TEntity)null)
                throw new ArgumentNullException("item", Resources.Exception_ItemArgumentIsNull);

            DbSet<TEntity> objectSet = GetSet();
            objectSet.Attach(item);

            DbContext ctx = this.m_UnitOfWork as DbContext;
            ctx.Entry<TEntity>(item).State = EntityState.Modified;

        }


        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Domain.POCO.Models.IRepository{TEntity}"/>
        /// </summary>              
        public override void Dispose()
        {
            base.Dispose();
        }
        #endregion

    }
}
