using Kashef.Common.Utilities.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;



namespace Kashef.Common.Utilities.Diagnostics.Trace
{
    /// <summary>
    /// Trace helper for application's logging
    /// </summary>
    public sealed class TraceManager
        : ITraceManager
    {
        #region -- Local Variables --

        private TraceSource m_Source;

        #endregion

        #region  -- Constructor --

        /// <summary>
        /// Create a new instance of this trace manager
        /// </summary>
        public TraceManager()
        {
            // Create default source
            if (!EventLog.SourceExists("Promotion Service"))
                EventLog.CreateEventSource("Promotion Service", "Application");

            m_Source = new TraceSource("Promotion Service");

        }

        #endregion

        #region -- Private Methods --

        /// <summary>
        /// Trace internal message in configured listeners
        /// </summary>
        /// <param name="eventType">Event type to trace</param>
        /// <param name="message">Message of event</param>
        void TraceInternal(EventLogEntryType eventType, string message)
        {
            if (m_Source != null)
            {
                try
                {
                    //m_Source.TraceEvent(eventType, (int)eventType, message);

                    EventLog eventLog = new EventLog();

                    EventLog.WriteEntry(m_Source.Name, message, eventType);
                }
                catch (SecurityException)
                {
                    //Cannot access to file listener or cannot have
                    //privileges to write in event log
                    //do not propagete this :-(
                }
            }


        }
        #endregion

        #region -- Public Methods --

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Diagnostics.Trace.ITraceManager"/>
        /// </summary>
        /// <param name="operationName"><see cref="Kashef.Common.Utilities.Diagnostics.Trace.ITraceManager"/></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2135:SecurityRuleSetLevel2MethodsShouldNotBeProtectedWithLinkDemandsFxCopRule"),
        SecurityPermission(SecurityAction.LinkDemand)]
        public void TraceStartLogicalOperation(string operationName)
        {
            if (String.IsNullOrEmpty(operationName))
                throw new ArgumentNullException("operationName", Resources.Exception_InvalidTraceMessage);

            System.Diagnostics.Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            System.Diagnostics.Trace.CorrelationManager.StartLogicalOperation(operationName);
        }

        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Diagnostics.Trace.ITraceManager"/>
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2135:SecurityRuleSetLevel2MethodsShouldNotBeProtectedWithLinkDemandsFxCopRule"),
        SecurityPermission(SecurityAction.LinkDemand)]
        public void TraceStopLogicalOperation()
        {
            try
            {
                System.Diagnostics.Trace.CorrelationManager.StopLogicalOperation();
            }
            catch (InvalidOperationException)
            {
                //stack empty
            }
        }
        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Diagnostics.Trace.ITraceManager"/>
        /// </summary>
        public void TraceStart()
        {
            TraceInternal(EventLogEntryType.Information, Resources.Constant_START);
        }
        /// <summary>
        ///<see cref="Kashef.Common.Utilities.Diagnostics.Trace.ITraceManager"/>
        /// </summary>
        public void TraceStop()
        {
            TraceInternal(EventLogEntryType.Information, Resources.Constant_STOP);
        }
        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Diagnostics.Trace.ITraceManager"/>
        /// </summary>
        /// <param name="message"><see cref="Kashef.Common.Utilities.Diagnostics.Trace.ITraceManager"/></param>
        public void TraceInfo(string message)
        {
            if (String.IsNullOrEmpty(message))
                throw new ArgumentNullException("message", Resources.Exception_InvalidTraceMessage);

            TraceInternal(EventLogEntryType.Information, message);

        }
        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Diagnostics.Trace.ITraceManager"/>
        /// </summary>
        /// <param name="message"><see cref="Kashef.Common.Utilities.Diagnostics.Trace.ITraceManager"/></param>
        public void TraceWarning(string message)
        {
            if (String.IsNullOrEmpty(message))
                throw new ArgumentNullException("message", Resources.Exception_InvalidTraceMessage);

            TraceInternal(EventLogEntryType.Warning, message);

        }
        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Diagnostics.Trace.ITraceManager"/>
        /// </summary>
        /// <param name="message"><see cref="Kashef.Common.Utilities.Diagnostics.Trace.ITraceManager"/></param>
        public void TraceError(string message)
        {
            if (String.IsNullOrEmpty(message))
                throw new ArgumentNullException("message", Resources.Exception_InvalidTraceMessage);

            TraceInternal(EventLogEntryType.Error, message);

        }
        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Diagnostics.Trace.ITraceManager"/>
        /// </summary>
        /// <param name="message"><see cref="Kashef.Common.Utilities.Diagnostics.Trace.ITraceManager"/></param>
        public void TraceCritical(string message)
        {
            if (String.IsNullOrEmpty(message))
                throw new ArgumentNullException("message", Resources.Exception_InvalidTraceMessage);

            TraceInternal(EventLogEntryType.Error, message);
        }
        /// <summary>
        /// <see cref="Kashef.Common.Utilities.Diagnostics.Trace.ITraceManager"/>
        /// </summary>
        /// <param name="exception"><see cref="Kashef.Common.Utilities.Diagnostics.Trace.ITraceManager"/></param>
        public void TraceError(Exception exception)
        {
            TraceInternal(EventLogEntryType.Error, string.Format("{0} {1}", exception.Message, exception.ToString()));
        }
        #endregion

    }
}
