using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public partial class LogsHandler(AppPreferences appPreferences, IConfirmationPrompt questionPrompt) : ObservableObject
    {
        private readonly AppPreferences appPreferences = appPreferences;
        private readonly IConfirmationPrompt prompt = questionPrompt;

        public void AddEntry(LogEntry entry)
        {
            if (appPreferences.LogWhenTimerFinishes == false)
                return;
            
            appPreferences.LogEntries.AddFirst(entry);
        }
        [RelayCommand]
        public async Task Clear()
        {
            var answer = await prompt.Show("Delete log?", "Do you want to delete all logs?", "OK", "Cancel");
            if (answer == false)
                return;

            appPreferences.LogEntries.Clear();
        }
    }
}
