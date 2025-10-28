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
        private readonly NotificationManagerWrapper notificationManagerWrapper;
        private readonly AppPreferences appPreferences;
        private readonly LogsHandler logsHandler;

        public event EventHandler? TimerStoppedOrFinished;

        public SleepTimerOrchestrator(
            MainTimer timer,
            AppPreferences preferences,
            //IVolumeService volumeService,
            //IMediaControlService mediaService,
            MediaController mediaController,
            MainPageDisplay display,
            NotificationManagerWrapper notificationManagerWrapper,
            LogsHandler logsHandler)
        {
            this.mainTimer = timer;
            this.appPreferences = preferences;
            this.mediaController = mediaController;
            this.mainPageDisplay = display;
            this.notificationManagerWrapper = notificationManagerWrapper;
            this.logsHandler = logsHandler;

            this.notifier = new TimerNotifier(preferences, (msg, level) => notificationManagerWrapper.Update(msg, level));

            //this.mediaController = new MediaController(volumeService, mediaService, preferences);
            //this.notifier = new TimerNotifier(preferences, (msg, level) => notificationManager.Update(msg, level));

            WireUpEvents();
        }

        private void WireUpEvents()
        {
            mainPageDisplay.SetStartTime(appPreferences.TimerDurationMinutes);

            mainTimer.Started += OnStarted;
            mainTimer.Tick += OnTick;
            mainTimer.EnteredStandby += OnEnterStandby;
            mainTimer.Finished += OnFinished;
            mainTimer.Extended += OnExtended;
        }

        public void HandleIntent(Intent? intent)
        {
            if (intent?.Action == ServiceAction.Start.ToString())
            {
                mainTimer.StartTimer(notificationManagerWrapper.Update);
                notificationManagerWrapper.Show($"Starting timer. {appPreferences.TimerDurationMinutes} minutes left.");
            }
            else if (intent?.Action == ServiceAction.Extend.ToString())
            {
                mainTimer.Extend();
                mediaController.RestoreVolume();
            }
            else if (intent?.Action == ServiceAction.Stop.ToString())
            {
                mainTimer.StopTimer();
                TimerStoppedOrFinished?.Invoke(this, EventArgs.Empty);
                mediaController.RestoreVolume();
            }
        }

        public void Cleanup()
        {
            mainTimer.StopTimer();
            //TimerStoppedOrFinished?.Invoke(this, EventArgs.Empty);
            mainTimer.Started -= OnStarted;
            mainTimer.Tick -= OnTick;
            mainTimer.EnteredStandby -= OnEnterStandby;
            mainTimer.Finished -= OnFinished;
            mainTimer.Extended -= OnExtended;
        }
        private void OnStarted(object? s,TimeSpan starting)
        {
            notifier.OnStart(starting);
            mediaController.SetStartingVolume();
            logsHandler.AddEntry(new LogEntryStarted());
        }
        private void OnTick(object? s, TimeSpan remaining)
        {
            //logsHandler.AddEntry($"Timer ticked [remaining {remaining.TotalSeconds}]");
            notifier.OnTick(remaining);
            mainPageDisplay.OnTick(remaining);

            if (remaining.TotalSeconds <= appPreferences.FadeOutSeconds)
                mediaController.HandleFadeOut(remaining);
            else
                mediaController.SetStartingVolume(); // refreshes every second in case user changed the volume. Stops updating only after the we are in the fade-out period.
        }
        private void OnExtended(object? sender, EventArgs e)
        {
            logsHandler.AddEntry(new LogEntryExtended());
        }
        private void OnEnterStandby(object? s, EventArgs e)
        {
            mediaController.EnterStandby();
            logsHandler.AddEntry(new LogEntryEnteringStandby());
        }
        private void OnFinished(object? s, EventArgs e)
        {
            TimerStoppedOrFinished?.Invoke(this, EventArgs.Empty);
            logsHandler.AddEntry(new LogEntryTimerFinished());
            mediaController.HandleFinished();
            mainTimer.StopTimer();
        }
    }
}