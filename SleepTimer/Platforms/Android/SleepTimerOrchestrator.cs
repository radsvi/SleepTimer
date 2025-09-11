using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using AndroidX.Core.App;

namespace SleepTimer.Platforms.Android
{
    public class SleepTimerOrchestrator
    {
        private readonly MainTimer mainTimer;
        private readonly MediaController mediaController;
        private readonly TimerNotifier notifier;
        private readonly MainPageDisplay mainPageDisplay;
        private readonly INotificationManager notificationManager;
        private readonly AppPreferences appPreferences;

        public event EventHandler? TimerStoppedOrFinished;

        public SleepTimerOrchestrator(
            MainTimer timer,
            AppPreferences preferences,
            IVolumeService volumeService,
            IMediaControlService mediaService,
            MainPageDisplay display,
            INotificationManager notificationManager)
        {
            this.mainTimer = timer;
            this.appPreferences = preferences;
            this.mediaController = new MediaController(volumeService, mediaService, preferences);
            this.notifier = new TimerNotifier(preferences, (msg, level) => notificationManager.Update(msg, level));
            this.mainPageDisplay = display;
            this.notificationManager = notificationManager;

            WireUpEvents();
        }

        private void WireUpEvents()
        {
            mainPageDisplay.SetStartTime(appPreferences.TimerDurationSeconds);

            mainTimer.Tick += (s, remaining) =>
            {
                notifier.OnTick(remaining);
                mainPageDisplay.OnTick(remaining);

                if (remaining.TotalSeconds <= appPreferences.FadeOutSeconds)
                    mediaController.HandleFadeOut(remaining);
                else
                    mediaController.SetStartingVolume(); // refreshes every second in case user changed the volume. Stops updating only after the we are in the fade-out period.
            };

            mainTimer.EnteredStandby += (s, e) => mediaController.EnterStandby();
            mainTimer.Finished += (s, e) =>
            {
                mainTimer.StopTimer();
                mediaController.HandleFinished();
                TimerStoppedOrFinished?.Invoke(this, EventArgs.Empty);
            };
        }

        public void HandleIntent(Intent? intent)
        {
            if (intent?.Action == ServiceAction.Start.ToString())
            {
                mainTimer.StartTimer(notificationManager.Update);
                notificationManager.Show($"Starting timer. {appPreferences.TimerDurationSeconds} minutes left.");
            }
            else if (intent?.Action == ServiceAction.Extend.ToString())
            {
                mainTimer.Extend();
            }
            else if (intent?.Action == ServiceAction.Stop.ToString())
            {
                mainTimer.StopTimer();
            }
        }

        public void Cleanup()
        {
            mainTimer.StopTimer();
            TimerStoppedOrFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}
