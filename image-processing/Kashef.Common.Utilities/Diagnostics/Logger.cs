using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.Diagnostics
{
    public static class Logger
    {
        public static void LogException(Exception ex)
        {
            System.Diagnostics.EventLog.WriteEntry("Application", ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
        }

        public static void LogInfo(string infoMessage)
        {
            System.Diagnostics.EventLog.WriteEntry("Application", infoMessage, System.Diagnostics.EventLogEntryType.Information);
        }
    }
}