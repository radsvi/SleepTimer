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

        public System.Timers.Timer? Timer { get; private set; }
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
            set { remainingTime = value; OnPropertyChanged(); OnPropertyChanged(nameof(DisplayRemainingTime)); }
        }

        public event EventHandler? Finished;
        public event EventHandler? EnteredStandby;
        public event EventHandler<TimeSpan>? Tick;


        //private bool isFinished;
        //public bool IsFinished
        //{
        //    get => isFinished;
        //    set { isFinished = value; OnPropertyChanged(); }
        //}
        private bool inStandby;
        public bool InStandby
        {
            get => inStandby;
            set { inStandby = value; OnPropertyChanged(); }
        }
        public TimeSpan DisplayRemainingTime { get => (EndTime != null) ? ((DateTime)EndTime - DateTime.Now) : throw new NullReferenceException(nameof(EndTime)); }

        public MainTimer(AppPreferences appPreferences)
        {
            this.appPreferences = appPreferences;
        }

        public void StartTimer(Action<string, NotificationLevel>? callback = null)
        {
            Timer = new System.Timers.Timer();
            Timer.Interval = 1000; // 1 second
            Timer.Elapsed += OnTick;
            EndTime = DateTime.Now.AddMinutes(appPreferences.DefaultDuration);

            IsStarted = true;
            Timer.Start();

            //this.callbackNotificationMessage = callback;
            //IsFinished = false;
            //InStandby = false;
            //EndTime = DateTime.Now.AddMinutes(appPreferences.DefaultDuration);
            //RemainingTime = (DateTime)EndTime - DateTime.Now;
            //StartingVolume = volumeService.GetVolume();
            //LastNotificationUpdate = DateTime.Now;


            //await Notifications.Show(new NotificationMessageRemainingTime(RemainingTime.Minutes));
        }
        public void StopTimer()
        {
            Timer?.Stop();
            Timer?.Dispose();
            Timer = null;

            IsStarted = false;
            EndTime = null;
            //InStandby = false;

            //volumeService.SetVolume(StartingVolume);
            

            //Notifications.Cancel();
        }
        public void Extend()
        {
            if (EndTime == null)
                return;

            EndTime = EndTime.Value.AddMinutes(appPreferences.ExtensionLength);

            //InStandby = false;

            //volumeService.SetVolume(StartingVolume);
            //callbackNotificationMessage?.Invoke($"{RemainingTime.Minutes} minutes left.", NotificationLevel.Low);
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
                EnteredStandby?.Invoke(this, EventArgs.Empty);
        }
    }
}
