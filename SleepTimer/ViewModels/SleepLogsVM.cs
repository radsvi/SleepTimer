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
        public LogsHandler LogsHandler { get; }
        public SleepLogsVM(AppPreferences appPreferences, LogsHandler logsHandler)
        {
            AppPreferences = appPreferences;
            LogsHandler = logsHandler;
        }
    }
}