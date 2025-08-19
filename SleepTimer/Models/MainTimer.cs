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
        readonly IVolumeService volumeService;
        public MainTimer(AppPreferences appPreferences, IVolumeService volumeService)
        {
            this.appPreferences = appPreferences;
            this.volumeService = volumeService;

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
        private bool isFinished;
        public bool IsFinished
        {
            get => isFinished;
            set { isFinished = value; OnPropertyChanged(); }
        }
        public int LastNotificationUpdate { get; private set; }
        private TimeSpan remainingTime = TimeSpan.MinValue;
        public TimeSpan RemainingTime
        {
            get => remainingTime;
            set { remainingTime = value; OnPropertyChanged(); }
        }
        public int StartingVolume { get; private set; }
        [Obsolete]private int volumeTest = 90;
        [Obsolete]public int VolumeTest
        {
            get => volumeTest;
            set { volumeTest = value; OnPropertyChanged(); }
        }
        private async void OnTimedEvent(object? source, ElapsedEventArgs e)
        {
            if (EndTime == null)
                return;

            RemainingTime = (DateTime)EndTime - e.SignalTime;

            if (RemainingTime.CompareTo(new TimeSpan(0,0,-Constants.ExtensionPeriod)) < 0)
            {
                IsFinished = true;
                volumeService.SetVolume(StartingVolume);
                Stop();
            }
            else if(RemainingTime.CompareTo(new TimeSpan(0, 0, Constants.FinalPhaseSeconds)) < 0)
            {
                DecreaseVolume();
            }
            else
            {
                // User can change the volume while the SleepTimer is active, but before the final phase is reached, and the starting volume is still being stored correctly.
                StartingVolume = volumeService.GetVolume();
            }

            if (RemainingTime.Minutes == 0 && RemainingTime.Seconds < 10)
            {
                await Notifications.Show(NotificationMsg.GoingToSleep);
            }
            else if (RemainingTime.Seconds > 0 && Math.Abs(RemainingTime.Minutes - LastNotificationUpdate) > 0)
            {
                await Notifications.Show(NotificationMsg.RemainingTime, RemainingTime.Minutes);
                LastNotificationUpdate = RemainingTime.Minutes;
            }
        }
        public async Task Start()
        {
            IsFinished = false;
            IsStarted = true;
            EndTime = DateTime.Now.AddMinutes(appPreferences.DefaultDuration);
            StartingVolume = volumeService.GetVolume();
            Timer.Enabled = true;

            if (EndTime != null)
                RemainingTime = (DateTime)EndTime - DateTime.Now;

            LastNotificationUpdate = RemainingTime.Minutes;
            await Notifications.Show(NotificationMsg.RemainingTime, RemainingTime.Minutes);
        }
        public void Stop()
        {
            Timer.Enabled = false;
            IsStarted = false;
            EndTime = null;

            Notifications.Cancel();
        }
        public void Extend()
        {
            if (EndTime is null) throw new InvalidOperationException();

            DateTime dateTime = (DateTime)EndTime;

            EndTime = dateTime.AddMinutes(appPreferences.ExtensionLength);

            volumeService.SetVolume(StartingVolume);
        }
        private void DecreaseVolume()
        {
            var currentVolume = volumeService.GetVolume();
            if (currentVolume <= 0)
                return;

            //var newVolume = currentVolume - Constants.VolumeStep;
            int newVolume = (StartingVolume * RemainingTime.Seconds / Constants.FinalPhaseSeconds);
            volumeService.SetVolume(newVolume);





            VolumeTest = newVolume;
        }
    }
}
