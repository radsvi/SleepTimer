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
        }
    }
}
