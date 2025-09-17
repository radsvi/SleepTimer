using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public class LogsHandler(AppPreferences appPreferences)
    {
        private readonly AppPreferences appPreferences = appPreferences;

        public void AddEntry(string message)
        {
            if (appPreferences.LogWhenTimerFinishes == false)
                return;
            
            appPreferences.LogEntries ??= [];

            appPreferences.LogEntries.Add(new LogEntry(DateTime.Now, message));
        }
    }
}
