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
        private readonly IGradualVolumeService volumeService;
        private readonly IMediaControlService mediaService;

        public MainTimer(AppPreferences appPreferences, IGradualVolumeService volumeService, IMediaControlService mediaService)
        {
            Debug.WriteLine("Reached MainTimer");
            this.appPreferences = appPreferences;
            this.volumeService = volumeService;
            this.mediaService = mediaService;
            

            //Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            Timer = new System.Timers.Timer();
            Timer.Elapsed += OnTimedEvent;
            Timer.Interval = 1000; // seconds
        }
        private Action<string>? callbackNotificationMessage;
        public System.Timers.Timer Timer { get; private set; }
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
        private int StartingVolume { get; set; }
        private void OnTimedEvent(object? source, ElapsedEventArgs e)
        {
            if (EndTime == null)
                return;

            RemainingTime = (DateTime)EndTime - e.SignalTime;

            if (RemainingTime.CompareTo(new TimeSpan(0, 0, -appPreferences.WaitTimeAfterFadeOut)) < 0)
            {
                IsFinished = true;
                mediaService.StopPlayback();
                volumeService.SetVolume(StartingVolume);
                StopTimer();

                //callbackNotificationMessage?.Invoke($"{RemainingTime.Minutes} minutes left.");
            }
            else if (RemainingTime.CompareTo(new TimeSpan(0, 0, 0)) == 0)
            {
                volumeService.SetVolume(0);
                mediaService.StopPlayback();

                //callbackNotificationMessage?.Invoke($"{RemainingTime.Minutes} minutes left.");
            }
            else if (RemainingTime.CompareTo(new TimeSpan(0, 0, Constants.FadeOutDuration)) < 0)
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

            if (RemainingTime.CompareTo(new TimeSpan(0, 0, -appPreferences.WaitTimeAfterFadeOut)) < 0)
            {
                callbackNotificationMessage?.Invoke($"Sleep timer finished.");
            }
            else if (RemainingTime.CompareTo(new TimeSpan(0, 0, 0)) == 0)
            {
                callbackNotificationMessage?.Invoke($"Sleeping. {appPreferences.WaitTimeAfterFadeOut} minutes wait time after fade out.");
            }
            else if (RemainingTime.Minutes == 0 && RemainingTime.Seconds < 10)
            {
                //await Notifications.Show(new NotificationMessageGoingToSleep());
                callbackNotificationMessage?.Invoke("Going to sleep.");
            }
            else if (RemainingTime.Minutes == 0 && RemainingTime.Seconds >= 10)
            {
                callbackNotificationMessage?.Invoke($"{RemainingTime.Seconds} seconds left.");
            }
            else if (RemainingTime.Seconds > 0 && Math.Abs(RemainingTime.Minutes - LastNotificationUpdate) > 0)
            {
                //await Notifications.Show(new NotificationMessageRemainingTime(RemainingTime.Minutes));
                callbackNotificationMessage?.Invoke($"{RemainingTime.Minutes} minutes left.");
                LastNotificationUpdate = RemainingTime.Minutes;
            }
        }
        //private async void OnTimedEvent(object? source, ElapsedEventArgs e)
        //{
        //    await sleepTimerService.OnTimedEvent(source, e);
        //}
        public void StartTimer(Action<string>? callback = null)
        {
            this.callbackNotificationMessage = callback;
            IsFinished = false;
            IsStarted = true;
#warning revert EndTime
            //EndTime = DateTime.Now.AddMinutes(appPreferences.DefaultDuration);
            EndTime = DateTime.Now.AddSeconds(20);
            StartingVolume = volumeService.GetVolume();
            Timer.Enabled = true;

            if (EndTime != null)
                RemainingTime = (DateTime)EndTime - DateTime.Now;

            LastNotificationUpdate = RemainingTime.Minutes;
            //await Notifications.Show(new NotificationMessageRemainingTime(RemainingTime.Minutes));
        }
        public void StopTimer()
        {
            Timer.Enabled = false;
            IsStarted = false;
            EndTime = null;

            volumeService.SetVolume(StartingVolume);

            //Notifications.Cancel();
        }
        public void Extend()
        {
            if (EndTime is null) throw new InvalidOperationException();

            DateTime dateTime = (DateTime)EndTime;

            EndTime = dateTime.AddMinutes(appPreferences.ExtensionLength);

            volumeService.SetVolume(StartingVolume);
        }
        private void GraduallyDecreaseVolume()
        {
            var currentVolume = volumeService.GetVolume();
            if (currentVolume <= 1)
                return;

            //var newVolume = currentVolume - Constants.VolumeStep;
            int newVolume = (StartingVolume * RemainingTime.Seconds / Constants.FadeOutDuration);
            volumeService.SetVolume(newVolume);
        }
    }
}
