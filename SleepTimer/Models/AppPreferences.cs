using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public partial class AppPreferences : AppPreferencesBase
    {
        private PreferencesObject<int> timerDurationMinutes = new(15);
        public int TimerDurationMinutes { get => timerDurationMinutes.Value; set => SetProperty(ref timerDurationMinutes, value); }

        private PreferencesObject<int> fadeOutSeconds = new(120);
        public int FadeOutSeconds { get => fadeOutSeconds.Value; set => SetProperty(ref fadeOutSeconds, value); }

        private PreferencesObject<int> extensionLengthMinutes = new(5);
        public int ExtensionLengthMinutes { get => extensionLengthMinutes.Value; set => SetProperty(ref extensionLengthMinutes, value); }

        private PreferencesObject<int> standBySeconds = new(60);
        public int StandBySeconds { get => standBySeconds.Value; set => SetProperty(ref standBySeconds, value); }

        private PreferencesObject<bool> displayVolumeChange = new(false);
        public bool DisplayVolumeChange { get => displayVolumeChange.Value; set => SetProperty(ref displayVolumeChange, value); }

        private PreferencesObject<bool> logWhenTimerFinishes = new(true);
        public bool LogWhenTimerFinishes { get => logWhenTimerFinishes.Value; set => SetProperty(ref logWhenTimerFinishes, value); }

        private PreferencesObject<LogTypes> logFilter = new(LogTypes.Finished);
        public LogTypes LogFilter { get => logFilter.Value; set => SetProperty(ref logFilter, value); }


        private PreferencesLinkedList<LogEntry> logEntries = new();
        public PreferencesLinkedList<LogEntry> LogEntries { get => logEntries; set => logEntries = value; }


    }
}
