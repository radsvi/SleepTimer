using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public partial class AppPreferences : ObservableObject
    {
        //public int TimerDurationSeconds
        //{
        //    get => Preferences.Default.Get(nameof(TimerDurationSeconds), 15);
        //    set { Preferences.Set(nameof(TimerDurationSeconds), value); OnPropertyChanged(); }
        //}
        public PreferencesObject<int> TimerDurationMinutes { get; set; } = new(15);
        //public int FadeOutSeconds
        //{
        //    get => Preferences.Default.Get(nameof(FadeOutSeconds), 120);
        //    set { Preferences.Set(nameof(FadeOutSeconds), value); OnPropertyChanged(); }
        //}
        public PreferencesObject<int> FadeOutSeconds { get; set; } = new(120);
        //public int ExtensionLength
        //{
        //    get => Preferences.Default.Get(nameof(ExtensionLength), 5);
        //    set { Preferences.Set(nameof(ExtensionLength), value); OnPropertyChanged(); }
        //}
        public PreferencesObject<int> ExtensionLength { get; set; } = new(5);
        //public int StandBySeconds // WaitTimeAfterFadeOut
        //{
        //    get => Preferences.Default.Get(nameof(StandBySeconds), 60);
        //    set { Preferences.Set(nameof(StandBySeconds), value); OnPropertyChanged(); }
        //}
        public PreferencesObject<int> StandBySeconds { get; set; } = new(60);
        //public bool DisplayVolumeChange
        //{
        //    get => Preferences.Default.Get(nameof(DisplayVolumeChange), false);
        //    set { Preferences.Set(nameof(DisplayVolumeChange), value); OnPropertyChanged(); }
        //}
        public PreferencesObject<bool> DisplayVolumeChange { get; set; } = new(false);
        //public bool LogWhenTimerFinishes
        //{
        //    get => Preferences.Default.Get(nameof(LogWhenTimerFinishes), false);
        //    set { Preferences.Set(nameof(LogWhenTimerFinishes), value); OnPropertyChanged(); }
        //}
        public PreferencesObject<bool> LogWhenTimerFinishes { get; set; } = new(false);
        public PreferencesObservableCollection<LogEntry> LogEntries { get; set; } = [];


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
