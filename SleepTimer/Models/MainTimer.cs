using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SleepTimer.Models
{
    public partial class MainTimer : ObservableObject
    {
        private readonly AppPreferences appPreferences;

        public System.Timers.Timer? timer;
        private bool isStarted;
        public bool IsStarted
        {
            get => isStarted;
            set { isStarted = value; OnPropertyChanged(); }
        }
        private TimeSpan? remainingTime = TimeSpan.MinValue;
        public TimeSpan? RemainingTime
        {
            get => remainingTime;
            private set { remainingTime = value; OnPropertyChanged(); }
        }

        public event EventHandler<TimeSpan>? Started;
        public event EventHandler? Finished;
        public event EventHandler? EnteredStandby;
        public event EventHandler<TimeSpan>? Tick;
        public event EventHandler? Extended;

        private bool inStandby = false;
        public bool InStandby
        {
            get => inStandby;
            private set { inStandby = value; OnPropertyChanged(); }
        }

        public MainTimer(AppPreferences appPreferences)
        {
            this.appPreferences = appPreferences;
        }

        public void StartTimer(Action<string, NotificationLevel>? callback = null)
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000; // 1 second
            timer.Elapsed += OnTick;

            RemainingTime = new TimeSpan(0, appPreferences.TimerDurationMinutes, 0);
            InStandby = false;
            IsStarted = true;
            timer.Start();
            Started?.Invoke(this, new TimeSpan(0,0, appPreferences.TimerDurationMinutes));
        }
        public void StopTimer()
        {
            timer?.Stop();
            timer?.Dispose();
            timer = null;

            IsStarted = false;
            RemainingTime = null;
            InStandby = false;
        }
        public void Extend()
        {
            if (RemainingTime == null)
                return;

            RemainingTime = RemainingTime.Value.Add(new TimeSpan(0, appPreferences.ExtensionLengthMinutes, 0));

            InStandby = false;
            Extended?.Invoke(this, EventArgs.Empty);
        }
        private void OnTick(object? source, ElapsedEventArgs e)
        {
            if (RemainingTime == null)
                return;

            RemainingTime = RemainingTime.Value.Subtract(TimeSpan.FromSeconds(1));
            Tick?.Invoke(this, RemainingTime.Value);

            if (RemainingTime.Value.TotalSeconds + appPreferences.StandBySeconds <= 0)
                Finished?.Invoke(this, EventArgs.Empty);
            else if (RemainingTime.Value.TotalSeconds <= 0 && InStandby == false)
            {
                InStandby = true;
                EnteredStandby?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
