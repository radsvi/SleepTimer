using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public partial class LogsHandler : ObservableObject
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

            appPreferences.LogEntries.AddFirst(new LogEntry(DateTime.Now, message));
        }
        [RelayCommand]
        public void Clear()
        {
            appPreferences.LogEntries.Clear();
            OnPropertyChanged(nameof(appPreferences.LogEntries));
        }
    }
}
