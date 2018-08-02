using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.ServiceModel.Audit
{  
    /// <summary>
    /// A WCF service behavior that inspects every WCF message call and logs it into target database.
    /// </summary>
    public class WcfMessageLogger : Attribute, IDispatchMessageInspector, IServiceBehavior
    {
        #region -- Local Varaibles --
        private readonly string m_ConnectionString;
        private readonly long m_MaxMessageSize;
        private readonly bool m_LogRequestOnly;
        private readonly string m_EventLogSource;
        private readonly bool m_IsEnabled;
        private readonly string m_EventLogName;
        #endregion

        #region -- Properties --
        /// <summary>
        /// Connection string for target database that is used for logging WCF messages.
        /// </summary>
        public string ConnectionString
        {
            get { return m_ConnectionString; }
        }

        /// <summary>
        /// Defines the max message size to be logged.
        /// </summary>
        public long MaxMessageSize
        {
            get { return m_MaxMessageSize; }
        }

        /// <summary>
        /// Indicates that only WCF requests will be logged.
        /// </summary>
        public bool LogRequestOnly
        {
            get { return m_LogRequestOnly; }
        }

        /// <summary>
        /// Defines the Windows Event Log to be used in case of errors during logging.
        /// </summary>
        public string EventLogSource
        {
            get { return m_EventLogSource; }
        }

        /// <summary>
        /// Indicates if the WCF audit is active or not.
        /// </summary>
        public bool IsEnabled
        {
            get { return m_IsEnabled; }
        }

        /// <summary>
        /// Define the Windows Event Log Name
        /// </summary>
        public string EventLogName
        {
            get { return m_EventLogName; }
        }

        #endregion

        #region -- Constructor --
        /// <summary>
        /// WCF logger constructor.
        /// </summary>
        /// <param name="Enabled">Indicates if the WCF audit is active or not.</param>
        /// <param name="LogConnectionString">Connection string for target database that is used for logging WCF messages.</param>
        /// <param name="MaxMessageSize">Max message size to be logged.</param>
        /// <param name="LogRequestOnly">Indicates that only WCF requests will be logged.</param>
        /// <param name="EventLogSource">Logger Source Name.</param>
        /// <param name="EventLogName">Defines the Windows Event Log to be used in case of errors during logging.</param>
        public WcfMessageLogger(bool Enabled, string LogConnectionString, long MaxMessageSize, bool LogRequestOnly, string EventLogSource, string EventLogName)
        {
            this.m_IsEnabled = Enabled;
            this.m_ConnectionString = LogConnectionString;
            this.m_MaxMessageSize = MaxMessageSize;
            this.m_LogRequestOnly = LogRequestOnly;
            this.m_EventLogSource = EventLogSource;
            this.m_EventLogName = EventLogName;
        }
        #endregion

        #region -- IDispatchMessageInspector --

        /// <summary>
        /// <see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/>
        /// </summary>
        /// <param name="request"><see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/></param>
        /// <param name="channel"><see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/></param>
        /// <param name="instanceContext"><see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/></param>
        /// <returns><see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/></returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel,
            InstanceContext instanceContext)
        {
            if (IsEnabled)
            {
                OperationContext context = OperationContext.Current;
                MessageBuffer buffer = request.CreateBufferedCopy(int.MaxValue);
                request = buffer.CreateMessage();
                if (context != null && buffer.BufferSize <= this.MaxMessageSize)
                {
                    try
                    {
                         
                        Message tmpMesage = buffer.CreateMessage();
                        Process CurrentProcess = Process.GetCurrentProcess();
                        IIdentity SourceIdentity = GenericPrincipal.Current.Identity;
                        MessageProperties messageProperties = context.IncomingMessageProperties;
                        RemoteEndpointMessageProperty endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                        IIdentity ClientIdentity = null;
                        if (context.ServiceSecurityContext != null)
                            ClientIdentity = context.ServiceSecurityContext.PrimaryIdentity;
                        WcfEvent wcfEvent = new WcfEvent()
                        {
                            
                            MessageID = Guid.NewGuid(),                            
                            ProcessID = CurrentProcess.Id,
                            ThreadID = Thread.CurrentThread.ManagedThreadId,
                            TimeCreated = DateTime.Now,
                            ServiceName = context.Host.Description.Name,
                            ServiceMachineName = Environment.MachineName,
                            ServiceUri = context.Channel.LocalAddress.ToString(),
                            ServiceIP = context.Channel.LocalAddress.Uri.Host,
                            ServicePort = context.Channel.LocalAddress.Uri.Port,
                            ServiceIdentity = SourceIdentity.Name,
                            ServiceAuthenticationType = SourceIdentity.AuthenticationType,
                            ClientIP = endpointProperty.Address,
                            ClientPort = endpointProperty.Port,
                            ClientIdentity = ClientIdentity == null ? "Anonymous" : ClientIdentity.Name,
                            ClientAuthenticationType = ClientIdentity == null ? "None" : ClientIdentity.AuthenticationType,
                            Action = request.Headers.Action,
                            Request = tmpMesage.GetReaderAtBodyContents().ReadOuterXml(),
                            Response = null,
                            Misc = null,
                            IsFault = request.IsFault
                        };
                        return wcfEvent;
                    }
                    catch (Exception ex)
                    {
                        if (!System.Diagnostics.EventLog.SourceExists(this.EventLogSource))
                        {
                            System.Diagnostics.EventLog.CreateEventSource(this.EventLogSource,this.EventLogName);
                        }
                        System.Diagnostics.EventLog.WriteEntry(this.EventLogSource, ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// <see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/>
        /// </summary>
        /// <param name="reply"><see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/></param>
        /// <param name="correlationState"><see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/></param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (correlationState != null && correlationState.GetType() == typeof(WcfEvent))
            {
                WcfEvent wcfEvent = (WcfEvent)correlationState;
                wcfEvent.IsFault = reply.IsFault;
                if (this.LogRequestOnly && wcfEvent.Request.Length <= this.MaxMessageSize)
                    Task.Factory.StartNew(() => AddWcfEvent((WcfEvent)correlationState));
                else
                {
                    MessageBuffer buffer = reply.CreateBufferedCopy(int.MaxValue);
                    reply = buffer.CreateMessage();
                    if (buffer.BufferSize + wcfEvent.Request.Length <= this.MaxMessageSize)
                    {
                        wcfEvent.Response = buffer.CreateMessage().GetReaderAtBodyContents().ReadInnerXml();
                        Task.Factory.StartNew(() => AddWcfEvent(wcfEvent));
                    }
                }
            }
        }

        #endregion

        #region -- IServiceBehavior --
        /// <summary>
        /// <see cref="System.ServiceModel.Description.IServiceBehavior"/>
        /// </summary>
        /// <param name="serviceDescription"><see cref="System.ServiceModel.Description.IServiceBehavior"/></param>
        /// <param name="serviceHostBase"><see cref="System.ServiceModel.Description.IServiceBehavior"/></param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase)
        {

            foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (var endpoint in dispatcher.Endpoints)
                {
                    endpoint.DispatchRuntime.MessageInspectors.Add(this);
                }
            }
        }

        /// <summary>
        /// <see cref="System.ServiceModel.Description.IServiceBehavior"/>
        /// </summary>
        /// <param name="serviceDescription"><see cref="System.ServiceModel.Description.IServiceBehavior"/></param>
        /// <param name="serviceHostBase"><see cref="System.ServiceModel.Description.IServiceBehavior"/></param>
        /// <param name="endpoints"><see cref="System.ServiceModel.Description.IServiceBehavior"/></param>
        /// <param name="bindingParameters"><see cref="System.ServiceModel.Description.IServiceBehavior"/></param>
        public void AddBindingParameters(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// <see cref="System.ServiceModel.Description.IServiceBehavior"/>
        /// </summary>
        /// <param name="serviceDescription"><see cref="System.ServiceModel.Description.IServiceBehavior"/></param>
        /// <param name="serviceHostBase"><see cref="System.ServiceModel.Description.IServiceBehavior"/></param>
        public void Validate(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase)
        {
        }
        #endregion

        #region -- Private Methods --
        private void AddWcfEvent(WcfEvent wcfEvent)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand objCommand = new SqlCommand();
                    objCommand.Connection = con;
                    objCommand.CommandText = "AddSSOManagerEvents";
                    objCommand.CommandType = CommandType.StoredProcedure;
                    objCommand.Parameters.AddRange(new SqlParameter[]{
                    new SqlParameter("@MessageID",wcfEvent.MessageID),                    
                    new SqlParameter("@ProcessID",wcfEvent.ProcessID),
                    new SqlParameter("@ThreadID",wcfEvent.ThreadID),
                    new SqlParameter("@TimeCreated",wcfEvent.TimeCreated),
                    new SqlParameter("@ServiceName",wcfEvent.ServiceName),
                    new SqlParameter("@ServiceMachineName",wcfEvent.ServiceMachineName),
                    new SqlParameter("@ServiceUri",wcfEvent.ServiceUri),
                    new SqlParameter("@ServiceIP",wcfEvent.ServiceIP),
                    new SqlParameter("@ServicePort",wcfEvent.ServicePort),
                    new SqlParameter("@ServiceIdentity",wcfEvent.ServiceIdentity),
                    new SqlParameter("@ServiceAuthenticationType",wcfEvent.ServiceAuthenticationType),
                    new SqlParameter("@ClientIP",wcfEvent.ClientIP),
                    new SqlParameter("@ClientPort",wcfEvent.ClientPort),
                    new SqlParameter("@ClientIdentity",wcfEvent.ClientIdentity),
                    new SqlParameter("@ClientAuthenticationType",wcfEvent.ClientAuthenticationType), 
                    new SqlParameter("@Action",wcfEvent.Action),
                    new SqlParameter("@IsFault",wcfEvent.IsFault)});

                    if (string.IsNullOrEmpty(wcfEvent.Request))
                        objCommand.Parameters.Add(new SqlParameter("@Request", DBNull.Value));
                    else
                        objCommand.Parameters.Add(new SqlParameter("@Request", wcfEvent.Request));

                    if (string.IsNullOrEmpty(wcfEvent.Response))
                        objCommand.Parameters.Add(new SqlParameter("@Response", DBNull.Value));
                    else
                        objCommand.Parameters.Add(new SqlParameter("@Response", wcfEvent.Response));

                    if (string.IsNullOrEmpty(wcfEvent.Misc))
                        objCommand.Parameters.Add(new SqlParameter("@Misc", DBNull.Value));
                    else
                        objCommand.Parameters.Add(new SqlParameter("@Misc", wcfEvent.Misc)); 
                    con.Open();
                    objCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                if (!System.Diagnostics.EventLog.SourceExists(this.EventLogSource))
                {
                    System.Diagnostics.EventLog.CreateEventSource(this.EventLogSource, this.EventLogName,".");
                }
                System.Diagnostics.EventLog.WriteEntry(this.EventLogSource, ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }
        }
        #endregion

        #region -- Inner Classes --
        private class WcfEvent
        {
            public long EventID { get; set; }
            public System.Guid MessageID { get; set; }
            public System.Guid CorrelationID { get; set; }
            public int ThreadID { get; set; }
            public int ProcessID { get; set; }
            public System.DateTime TimeCreated { get; set; }
            public string ServiceName { get; set; }
            public string ServiceMachineName { get; set; }
            public string ServiceUri { get; set; }
            public string ServiceIP { get; set; }
            public int ServicePort { get; set; }
            public string ServiceIdentity { get; set; }
            public string ServiceAuthenticationType { get; set; }
            public string ClientIP { get; set; }
            public int ClientPort { get; set; }
            public string ClientIdentity { get; set; }
            public string ClientAuthenticationType { get; set; }
            public string Action { get; set; }
            public string Request { get; set; }
            public string Response { get; set; }
            public string Misc { get; set; }
            public Nullable<bool> IsFault { get; set; }
        }
        #endregion
    }

    /// <summary>
     /// A WCF service behavior extension that audit any WCF service calls into database.
    /// </summary>
    /// 
    public class WcfMessageLoggerExtension : BehaviorExtensionElement
    {
        /// <summary>
        /// Connection string for target database that is used for logging WCF messages.
        /// </summary>
        [ConfigurationProperty("ConnectionString", DefaultValue = "", IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)base["ConnectionString"]; }
            set { base["ConnectionString"] = value; }
        }

        /// <summary>
        /// Defines the max message size to be logged, default value is 100 MB.
        /// </summary>
        [ConfigurationProperty("MaxMessageSize", DefaultValue = "104857600", IsRequired = true)]
        public long MaxMessageSize
        {
            get { return long.Parse(base["MaxMessageSize"].ToString()); }
            set { base["MaxMessageSize"] = value; }
        }

        /// <summary>
        /// Indicates that only WCF requests will be logged. Default value is false.
        /// </summary>
        [ConfigurationProperty("LogRequestOnly", DefaultValue = "false", IsRequired = true)]
        public bool LogRequestOnly
        {
            get { return bool.Parse(base["LogRequestOnly"].ToString()); }
            set { base["LogRequestOnly"] = value; }
        }

        /// <summary>
        /// Indicates if the WCF audit is active or not.
        /// </summary>
        [ConfigurationProperty("Enabled", DefaultValue = "true", IsRequired = true)]
        public bool Enabled
        {
            get { return bool.Parse(base["Enabled"].ToString()); }
            set { base["Enabled"] = value; }
        }

        /// <summary>
        /// Defines the Windows Event Log to be used in case of errors during logging.
        /// </summary>
        [ConfigurationProperty("EventLogSource", DefaultValue = "SSOManagerLogger", IsRequired = true)]
        public string EventLogSource
        {
            get { return (string)base["EventLogSource"]; }
            set { base["EventLogSource"] = value; }
        }

        /// <summary>
        /// Defines the Windows Event Log to be used in case of errors during logging.
        /// </summary>
        [ConfigurationProperty("EventLogName", DefaultValue = "SSO Manager Logger", IsRequired = true)]
        public string EventLogName
        {
            get { return (string)base["EventLogName"]; }
            set { base["EventLogName"] = value; }
        }

        /// <summary>
        /// Gets the type of the behavior extension element. It returns <see cref="Kashef.Common.Utilities.ServiceModel.Audit.WcfMessageLogger"/>
        /// </summary>
        public override Type BehaviorType
        {
            get
            {
                return typeof(WcfMessageLogger);
            }
        }

        /// <summary>
        /// Creates the behavior, it creates object of type <see cref="Kashef.Common.Utilities.ServiceModel.Audit.WcfMessageLogger"/>
        /// </summary>
        /// <returns></returns>
        protected override object CreateBehavior()
        {
            return new WcfMessageLogger(Enabled, ConnectionString, MaxMessageSize, LogRequestOnly,EventLogSource,EventLogName);
        }        
    }        
}
