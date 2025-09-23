using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public partial class LogsHandler(AppPreferences appPreferences, QuestionPrompt questionPrompt) : ObservableObject
    {
        private readonly AppPreferences appPreferences = appPreferences;
        private readonly QuestionPrompt prompt = questionPrompt;

        public void AddEntry(string message)
        {
            if (appPreferences.LogWhenTimerFinishes == false)
                return;
            
            appPreferences.LogEntries.AddFirst(new LogEntry(DateTime.Now, message));
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
