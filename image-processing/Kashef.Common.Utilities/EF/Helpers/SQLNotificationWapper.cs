using Kashef.Common.Utilities.EF.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.EF.Helpers
{
    public class SQLNotificationWapper<TEntity> : IDisposable where TEntity : class
    { 
        #region Properties

        private ObjectQuery _objectQuery = null;

        private SqlCommand _command = null;

        private DbContext _context = null;

        private SqlDependency _dependency = null;

        public event EventHandler OnChanged;

        #endregion

        #region Constructors

        public SQLNotificationWapper(DbContext context)
        {
            //Precondition validation...
            if (null == context)
            {
                throw new ArgumentException("Paramter cannot be null", "context");
            }

            //Set properities values..
            _context = context;
        }


        /// <summary>
        /// Create instance from SQL Notification and run SQL dependency Mointoring...
        /// </summary>
        /// <param name="context"></param>
        /// <param name="query"></param>
        public SQLNotificationWapper(DbContext context, IQueryable query)
            : this(context)
        {

            if (null == query)
            {
                throw new ArgumentException("Paramter cannot be null", "query");
            }

            //SQL Watcher
            StartWatch();

            //Query Subscription... 
            Subscribe(query);
        }


        #endregion

        #region Public Methods

        /// <summary> 
        /// Starts the notification of SqlDependency  
        /// </summary> 
        /// <param name="context">An instance of dbcontext</param> 
        public void StartWatch()
        {
            try
            {
                SqlDependency.Start(_context.Database.Connection.ConnectionString);
            }
            catch (Exception ex)
            {
                throw new System.Exception("Fails to Start the SqlDependency in the RegularlyNotificationRegister class", ex);
            }
        }

        public void Subscribe(IQueryable query)
        {
            if (null == _dependency)
            {
                //Get Query Object from IQuery Interface Value...
                _objectQuery = QueryExtension.GetObjectQuery<TEntity>(_context, query);

                //Get SQL connection string from Query Context...
                string connection = QueryExtension.GetConnectionString(_context);

                //Get SQL Command from Object Query...
                string commandString = QueryExtension.GetSqlString(_objectQuery);

                //Register SQL command
                using (SqlCommand command = new SqlCommand(commandString, new SqlConnection(connection)))
                {
                    //Open SQL Connection 
                    command.Connection.Open();

                    command.Notification = null;

                    command.NotificationAutoEnlist = true;

                    // Create a dependency and associate it with the SqlCommand.  
                    _dependency = new SqlDependency(command);
                    _dependency.OnChange += dependency_OnChange;

                    // Execute the command.  
                    SqlDataReader reader = command.ExecuteReader();
                }
            }
        }


        /// <summary> 
        /// Stops the notification of SqlDependency  
        /// </summary> 
        /// <param name="context">An instance of dbcontext</param> 
        public void StopWatch()
        {
            try
            {
                SqlDependency.Stop(_context.Database.Connection.ConnectionString);
            }
            catch (Exception ex)
            {
                throw new System.Exception("Fails to Stop the SqlDependency in the RegularlyNotificationRegister class", ex);
            }
        }


        /// <summary> 
        /// Releases all the resources by the RegularlyNotificationRegister. 
        /// </summary> 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposed)
        {
            if (disposed)
            {
                //Stop Watch
                if (null != _context)
                {
                    StopWatch();
                }

                //Unsubscripe to the event handler
                _dependency.OnChange -= new OnChangeEventHandler(dependency_OnChange);

                //Dispose all objects
                _dependency = null;
                _context = null;
                _command = null;
                _objectQuery = null;
            }
        }
        #endregion

        #region Event Handler
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
           // if (e.Type == SqlNotificationType.Change)
            {
                OnChanged(this, e);
                Dispose();
            }
        }

        #endregion

    }
}
