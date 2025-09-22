using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public partial class LogsHandler(AppPreferences appPreferences) : ObservableObject
    {
        private readonly AppPreferences appPreferences = appPreferences;

        public void AddEntry(string message)
        {
            if (appPreferences.LogWhenTimerFinishes == false)
                return;
            
            appPreferences.LogEntries.AddFirst(new LogEntry(DateTime.Now, message));
        }
        [RelayCommand]
        public void Clear()
        {
            appPreferences.LogEntries.Clear();
        }
    }
}
