using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SleepTimer.Models
{
    public class MainTimer : ObservableObject
    {
        readonly AppPreferences appPreferences;
        public MainTimer(AppPreferences appPreferences)
        {

            this.appPreferences = appPreferences;

            //Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            Timer.Elapsed += OnTimedEvent;
            Timer.Interval = 1000; // seconds
        }
        public System.Timers.Timer Timer { get; private set; } = new System.Timers.Timer();
        public DateTime? EndTime { get; private set; }
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
            set { remainingTime = value; OnPropertyChanged(); }
        }

        private void OnTimedEvent(object? source, ElapsedEventArgs e)
        {
            if (EndTime != null)
                RemainingTime = (DateTime)EndTime - e.SignalTime;
        }
        public void Start()
        {
            Timer.Enabled = true;
            IsStarted = true;
            EndTime = DateTime.Now.AddMinutes(appPreferences.DefaultDuration);

            if (EndTime != null)
                RemainingTime = (DateTime)EndTime - DateTime.Now;
        }
        public void Stop()
        {
            Timer.Enabled = false;
            IsStarted = false;
            EndTime = null;
        }
        public void Extend()
        {
            if (EndTime is null) throw new InvalidOperationException();

            DateTime dateTime = (DateTime)EndTime;

            EndTime = dateTime.AddMinutes(appPreferences.ExtensionLength);
        }
    }
}
