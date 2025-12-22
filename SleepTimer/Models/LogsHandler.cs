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
        private readonly IConfirmationPrompt prompt;
        public ObservableCollection<LogEntry> DisplayEntries { get; private set; }

        public LogsHandler(AppPreferences appPreferences, IConfirmationPrompt questionPrompt)
        {
            this.appPreferences = appPreferences;
            prompt = questionPrompt;
            DisplayEntries = new();
            //UpdateLegacyLogs();
            RefreshEntries();
        }

        public void AddEntry(LogEntry entry)
        {
            if (appPreferences.LogWhenTimerFinishes == false)
                return;
            
            appPreferences.LogEntries.AddFirst(entry);
        }
        [RelayCommand]
        public async Task SelectFilter()
        {
            string action = await Shell.Current.DisplayActionSheet(
                "Filter by entry type:",
                "Cancel",
                null,
                LogTypes.All.ToString(),
                LogTypes.Started.ToString(),
                LogTypes.Extended.ToString(),
                LogTypes.Standby.ToString(),
                LogTypes.Finished.ToString(),
                LogTypes.StartAndStop.ToString()
            );

            if (action == "Cancel")
                return;

            //await Shell.Current.DisplayAlert("You selected", action, "OK");

            if (System.Enum.TryParse(action, true, out LogTypes filterType))
                appPreferences.LogFilter = filterType;

            RefreshEntries();
        }
        public void RefreshEntries()
        {
            DisplayEntries.Clear();

            foreach (var entry in appPreferences.LogEntries)
            {
                if (appPreferences.LogFilter == LogTypes.All)
                {
                    DisplayEntries.Add(entry);
                }
                else if (appPreferences.LogFilter == LogTypes.StartAndStop)
                {
                    if (entry.Type == LogTypes.Started || entry.Type == LogTypes.Finished || entry.Type == LogTypes.Stopped)
                    {
                        DisplayEntries.Add(entry);
                    }
                }
                else
                {
                    if (entry.Type == appPreferences.LogFilter)
                    {
                        DisplayEntries.Add(entry);
                    }
                }
            }
        }
        //private void UpdateLegacyLogs()
        //{
        //    foreach (LogEntry entry in appPreferences.LogEntries)
        //    {
        //        switch (entry.Text)
        //        {
        //            case "Timer started":
        //                entry.Type = LogTypes.Started;
        //                break;
        //            case "Timer extended":
        //                entry.Type = LogTypes.Extended;
        //                break;
        //            case "Entering standby":
        //                entry.Type = LogTypes.EnteringStandby;
        //                break;
        //            case "Timer finished":
        //                entry.Type = LogTypes.TimerFinished;
        //                break;
        //        }
        //    }

        //    PreferencesLinkedList<LogEntry> tempList = new();
        //    foreach (var oldEntry in appPreferences.LogEntries)
        //    {
        //        tempList.AddLast(oldEntry);
        //    }

        //    appPreferences.LogEntries.Clear();
        //    foreach (var newEntry in tempList)
        //    {
        //        appPreferences.LogEntries.AddLast(newEntry);
        //    }

        //    appPreferences.LogEntries.SaveChanges();
        //    //appPreferences.LogEntries.Save();
        //}
        [RelayCommand]
        public async Task Clear()
        {
            var answer = await prompt.Show("Delete log?", "Do you want to delete all logs?", "OK", "Cancel");
            if (answer == false)
                return;

            appPreferences.LogEntries.Clear();

            RefreshEntries();
        }
    }
}
