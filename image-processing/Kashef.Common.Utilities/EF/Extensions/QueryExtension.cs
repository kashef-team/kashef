using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.EF.Extensions
{
    public static class QueryExtension
    {
        /// <summary> 
        /// Return the ObjectQuery directly or convert the DbQuery to ObjectQuery. 
        /// </summary> 
        public static ObjectQuery GetObjectQuery<TEntity>(DbContext context, IQueryable query) where TEntity : class
        {
            if (query is ObjectQuery)
            {
                return query as ObjectQuery;
            }

            if (context == null)
            {
                throw new ArgumentException("Paramter cannot be null", "context");
            }

            // Use the DbContext to create the ObjectContext 
            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
         
            // Use the DbSet to create the ObjectSet and get the appropriate provider. 
            IQueryable iqueryable = objectContext.CreateObjectSet<TEntity>() as IQueryable;
            IQueryProvider provider = iqueryable.Provider;

            // Use the provider and expression to create the ObjectQuery. 
            return provider.CreateQuery(query.Expression) as ObjectQuery;
        } 

        public static String GetConnectionString(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentException("Paramter cannot be null", "query");
            }

            return context.Database.Connection.ConnectionString;
        }

        /// <summary> 
        /// Use ObjectQuery to get the Sql string. 
        /// </summary> 
        public static String GetSqlString(ObjectQuery query)
        {
            if (query == null)
            {
                throw new ArgumentException("Paramter cannot be null", "query");
            }

            return query.ToTraceString();
        }

    }
}