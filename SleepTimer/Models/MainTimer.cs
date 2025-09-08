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
        private readonly IVolumeService volumeService;
        private readonly IMediaControlService mediaService;

        public MainTimer(AppPreferences appPreferences, IVolumeService volumeService, IMediaControlService mediaService)
        {
            Debug.WriteLine("Reached MainTimer");
            this.appPreferences = appPreferences;
            this.volumeService = volumeService;
            this.mediaService = mediaService;
            

            //Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            
        }
        private Action<string, NotificationLevel>? callbackNotificationMessage;
        public event EventHandler OnTimeFinished;
        public System.Timers.Timer? Timer { get; private set; }
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
        private bool inStandby;
        public bool InStandby
        {
            get => inStandby;
            set { inStandby = value; OnPropertyChanged(); }
        }
        public int LastNotificationUpdate { get; private set; } = int.MaxValue;
        private TimeSpan remainingTime = TimeSpan.MinValue;
        public TimeSpan RemainingTime
        {
            get => remainingTime;
            set { remainingTime = value; OnPropertyChanged(); }
        }
        private int StartingVolume { get; set; }
        private void OnTimedEvent(object? source, ElapsedEventArgs e)
        {
            if (EndTime == null)
                return;

            RemainingTime = (DateTime)EndTime - e.SignalTime;

            if (RemainingTime.CompareTo(new TimeSpan(0, 0, -appPreferences.StandByDuration)) < 0)
            {
                IsFinished = true;
                mediaService.StopPlayback();
                volumeService.SetVolume(StartingVolume);
                StopTimer();

                //callbackNotificationMessage?.Invoke($"{RemainingTime.Minutes} minutes left.");
                OnTimeFinished?.Invoke(this, EventArgs.Empty);
            }
            else if (RemainingTime.CompareTo(new TimeSpan(0, 0, 0)) <= 0)
            {
                volumeService.SetVolume(0);
                mediaService.StopPlayback();
                InStandby = true;

                //callbackNotificationMessage?.Invoke($"{RemainingTime.Minutes} minutes left.");
            }
            else if (RemainingTime.CompareTo(new TimeSpan(0, 0, appPreferences.FadeOutDuration)) < 0)
            {
                GraduallyDecreaseVolume();

                //callbackNotificationMessage?.Invoke($"{RemainingTime.Seconds} seconds left.");
            }
            else
            {
                // User can change the volume while the SleepTimer is active, but before the final phase is reached, and the starting volume is still being stored correctly.
                StartingVolume = volumeService.GetVolume();

                //callbackNotificationMessage?.Invoke($"{RemainingTime.Minutes} minutes left.");
            }

            // Notifications:
            //callback?.Invoke($"Elapsed: {RemainingTime} minute(s)");
            //callback?.Invoke($"{RemainingTime.Minutes} minutes left.");

            if (RemainingTime.CompareTo(new TimeSpan(0, 0, -appPreferences.StandByDuration)) < 0)
            {
                callbackNotificationMessage?.Invoke($"Sleep timer finished.", NotificationLevel.Low);
            }
            else if (RemainingTime.CompareTo(new TimeSpan(0, 0, 0)) == 0)
            {
                callbackNotificationMessage?.Invoke($"Sleeping. {appPreferences.StandByDuration} minutes wait time after fade out.", NotificationLevel.Low);
            }
            else if (RemainingTime.Minutes == 0 && RemainingTime.Seconds < 10)
            {
                //await Notifications.Show(new NotificationMessageGoingToSleep());
                callbackNotificationMessage?.Invoke("Going to sleep.", NotificationLevel.Low);
            }
            else if (RemainingTime.Minutes == 0 && RemainingTime.Seconds >= 10)
            {
                callbackNotificationMessage?.Invoke($"{RemainingTime.Seconds} seconds left.", NotificationLevel.Low);
            }
            else if (RemainingTime.Seconds <= 1)
            {
                var highPriorityMinutes = new int[] { 1,2,5,10 }; // minutes where the notification will popup, instead of staying in background
                NotificationLevel chosenPriority;
                if (highPriorityMinutes.Contains(RemainingTime.Minutes))
                    chosenPriority = NotificationLevel.High;
                else
                    chosenPriority = NotificationLevel.Low;

                //await Notifications.Show(new NotificationMessageRemainingTime(RemainingTime.Minutes));
                callbackNotificationMessage?.Invoke($"{RemainingTime.Minutes} minutes left.", chosenPriority);
                LastNotificationUpdate = RemainingTime.Minutes;
            }
        }
        //private async void OnTimedEvent(object? source, ElapsedEventArgs e)
        //{
        //    await sleepTimerService.OnTimedEvent(source, e);
        //}
        public void StartTimer(Action<string, NotificationLevel>? callback = null)
        {
            Timer = new System.Timers.Timer();
            Timer.Elapsed += OnTimedEvent;
            Timer.Interval = 1000; // seconds

            this.callbackNotificationMessage = callback;
            IsFinished = false;
            IsStarted = true;
            InStandby = false;
            EndTime = DateTime.Now.AddMinutes(appPreferences.DefaultDuration);
            StartingVolume = volumeService.GetVolume();
            Timer.Enabled = true;

            if (EndTime != null)
                RemainingTime = (DateTime)EndTime - DateTime.Now;

            //await Notifications.Show(new NotificationMessageRemainingTime(RemainingTime.Minutes));
        }
        public void StopTimer()
        {
            if (Timer == null)
                return;

            Timer.Enabled = false;
            IsStarted = false;
            EndTime = null;
            InStandby = false;

            volumeService.SetVolume(StartingVolume);
            Timer.Dispose();
            Timer = null;

            //Notifications.Cancel();
        }
        public void Extend()
        {
            if (EndTime is null)
                return;

            DateTime dateTime = (DateTime)EndTime;

            EndTime = dateTime.AddMinutes(appPreferences.ExtensionLength);
            InStandby = false;

            volumeService.SetVolume(StartingVolume);
        }
        private void GraduallyDecreaseVolume()
        {
            var currentVolume = volumeService.GetVolume();
            if (currentVolume <= 1)
                return;

            //var newVolume = currentVolume - Constants.VolumeStep;
            int newVolume = (StartingVolume * RemainingTime.Seconds / appPreferences.FadeOutDuration);
            volumeService.SetVolume(newVolume);
        }
    }
}
