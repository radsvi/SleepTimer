using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.ViewModels
{
    public partial class SleepLogsVM : ObservableObject
    {
        public AppPreferences AppPreferences { get; }
        public SleepLogsVM(AppPreferences appPreferences)
        {
            AppPreferences = appPreferences;

            //AppPreferences.LogEntries = new List<LogEntry>();

            //AppPreferences.LogEntries.Add(new LogEntry(DateTime.Now, "test1"));
            //AppPreferences.LogEntries.Add(new LogEntry(DateTime.Now, "test2"));
            //AppPreferences.LogEntries.Add(new LogEntry(DateTime.Now, "test3"));
            //AppPreferences.LogEntries.Add(new LogEntry(DateTime.Now, "test4"));
            //AppPreferences.LogEntries.Add(new LogEntry(DateTime.Now, "test5"));

            //AppPreferences.LogEntriesTest.Add(new LogEntry(DateTime.Now, "test1"));
            //AppPreferences.LogEntriesTest.Add(new LogEntry(DateTime.Now, "test2"));
            //AppPreferences.LogEntriesTest.Add(new LogEntry(DateTime.Now, "test3"));
            //AppPreferences.LogEntriesTest.Add(new LogEntry(DateTime.Now, "test4"));
            //AppPreferences.LogEntriesTest.Add(new LogEntry(DateTime.Now, "test5"));
        }

        //private List<LogEntry> logEntriesTest = [];
        //public List<LogEntry> LogEntriesTest
        //{
        //    get { return logEntriesTest; }
        //    private set
        //    {
        //        if (value != logEntriesTest)
        //        {
        //            logEntriesTest = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}
    }
}
