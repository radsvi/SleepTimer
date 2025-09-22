using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public class LogsHandler
    {
        private readonly AppPreferences appPreferences;

        public LogsHandler(AppPreferences appPreferences)
        {
            this.appPreferences = appPreferences;
        }

        public void AddEntry(string message)
        {
            if (appPreferences.LogWhenTimerFinishes == false)
                return;
            
            //appPreferences.LogEntries ??= [];

            appPreferences.LogEntries.Add(new LogEntry(DateTime.Now, message));
            appPreferences.LogEntriesTest.Add(new LogEntry(DateTime.Now, message));
        }
    }
}
