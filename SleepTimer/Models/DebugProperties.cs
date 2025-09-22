using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public partial class DebugProperties : ObservableObject
    {
        private readonly AppPreferences appPreferences;

        public bool DisplayShortTimer { get; set; } = false;


        public DebugProperties(AppPreferences appPreferences)
        {
            this.appPreferences = appPreferences;

#if DEBUG
            DisplayShortTimer = true;
#endif
        }

        [RelayCommand]
        void MakeTimeShorter()
        {
            appPreferences.TimerDurationMinutes = 0;
        }
        [RelayCommand]
        void MakeTimeLonger()
        {
            appPreferences.TimerDurationMinutes = 5;
        }

    }
}
