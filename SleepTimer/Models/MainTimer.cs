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
        public DateTime? EndTime { get; private set; } = DateTime.MinValue;
        private bool isStarted;
        public bool IsStarted
        {
            get => isStarted;
            set { isStarted = value; OnPropertyChanged(); }
        }
        private TimeSpan remainingTime = TimeSpan.MinValue;
        public TimeSpan RemainingTime
        {
            get => remainingTime;
            private set { remainingTime = value; OnPropertyChanged(); OnPropertyChanged(nameof(DisplayRemainingTime)); }
        }

        public event EventHandler<TimeSpan>? Started;
        public event EventHandler? Finished;
        public event EventHandler? EnteredStandby;
        public event EventHandler<TimeSpan>? Tick;

        private bool inStandby = false;
        public bool InStandby
        {
            get => inStandby;
            private set { inStandby = value; OnPropertyChanged(); }
        }
        public TimeSpan DisplayRemainingTime { get => (EndTime != null) ? ((DateTime)EndTime - DateTime.Now) : throw new NullReferenceException(nameof(EndTime)); }

        public MainTimer(AppPreferences appPreferences)
        {
            this.appPreferences = appPreferences;
        }

        public void StartTimer(Action<string, NotificationLevel>? callback = null)
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000; // 1 second
            timer.Elapsed += OnTick;

            EndTime = DateTime.Now.AddMinutes(appPreferences.TimerDurationMinutes);
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
            EndTime = null;
            InStandby = false;
        }
        public void Extend()
        {
            if (EndTime == null)
                return;

            EndTime = EndTime.Value.AddMinutes(appPreferences.ExtensionLength);

            InStandby = false;
        }
        private void OnTick(object? source, ElapsedEventArgs e)
        {
            if (EndTime == null)
                return;

            RemainingTime = (DateTime)EndTime - e.SignalTime;
            Tick?.Invoke(this, RemainingTime);

            if (RemainingTime.TotalSeconds + appPreferences.StandBySeconds <= 0)
                Finished?.Invoke(this, EventArgs.Empty);
            else if (RemainingTime.TotalSeconds <= 0)
            {
                InStandby = true;
                EnteredStandby?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
