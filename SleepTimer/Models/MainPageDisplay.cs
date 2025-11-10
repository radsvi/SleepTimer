using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public partial class MainPageDisplay : ObservableObject
    {
        public AppPreferences AppPreferences { get; }
        public MainTimer MainTimer { get; }

        private RemainingTime remainingTime = RemainingTime.Zero;
        public RemainingTime RemainingTime
        {
            get => remainingTime;
            set { remainingTime = value; OnPropertyChanged(); }
        }
        public string TextBelowTimer
        {
            get
            {
                if (MainTimer.InStandby) return $"Stand by ({AppPreferences.StandBySeconds} seconds)";
                else return "MINUTES REMAINING";
            }
        }
        public MainPageDisplay(AppPreferences appPreferences, MainTimer mainTimer)
        {
            AppPreferences = appPreferences;
            MainTimer = mainTimer;

            MainTimer.EnteredStandby += (s, e) => OnPropertyChanged(nameof(TextBelowTimer));
        }

        public void SetStartTime(int minutes)
        {
            RemainingTime = new RemainingTime(minutes, 0);
        }
        public void OnTick(TimeSpan remainingTime)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                RemainingTime = new RemainingTime(remainingTime);
            });
        }
    }
}
