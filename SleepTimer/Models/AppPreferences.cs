using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public partial class AppPreferences : ObservableObject
    {
        public int TimerDurationSeconds
        {
            get => Preferences.Default.Get(nameof(TimerDurationSeconds), 15);
            set { Preferences.Set(nameof(TimerDurationSeconds), value); OnPropertyChanged(); }
        }
        public int FadeOutSeconds
        {
            get => Preferences.Default.Get(nameof(FadeOutSeconds), 120);
            set { Preferences.Set(nameof(FadeOutSeconds), value); OnPropertyChanged(); }
        }
        public int ExtensionLength
        {
            get => Preferences.Default.Get(nameof(ExtensionLength), 5);
            set { Preferences.Set(nameof(ExtensionLength), value); OnPropertyChanged(); }
        }
        public int StandBySeconds // WaitTimeAfterFadeOut
        {
            get => Preferences.Default.Get(nameof(StandBySeconds), 60);
            set { Preferences.Set(nameof(StandBySeconds), value); OnPropertyChanged(); }
        }
        public bool DisplayVolumeChange
        {
            get => Preferences.Default.Get(nameof(DisplayVolumeChange), false);
            set { Preferences.Set(nameof(DisplayVolumeChange), value); OnPropertyChanged(); }
        }
        public bool LogWhenTimerFinishes
        {
            get => Preferences.Default.Get(nameof(LogWhenTimerFinishes), false);
            set { Preferences.Set(nameof(LogWhenTimerFinishes), value); OnPropertyChanged(); }
        }
    }
}
