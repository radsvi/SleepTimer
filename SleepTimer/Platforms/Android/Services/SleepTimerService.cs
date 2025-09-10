using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using AndroidX.Core.App;


namespace SleepTimer.Platforms.Android.Services
{
    //[Service(Enabled = true, ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeMediaPlayback)]
    [Service(ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeMediaPlayback)]
    public class SleepTimerService : Service, ISleepTimerService
    {
        const int SERVICE_ID = 1001;
        //private readonly AudioManager audioManager = (AudioManager?)global::Android.App.Application.Context.GetSystemService(AudioService) ?? throw new NullReferenceException(nameof(audioManager));
        private readonly AppPreferences appPreferences = ServiceHelper.GetService<AppPreferences>();
        private readonly MainTimer mainTimer = ServiceHelper.GetService<MainTimer>();
        private readonly MainPageDisplay mainPageDisplay = ServiceHelper.GetService<MainPageDisplay>();
        private readonly IVolumeService volumeService = ServiceHelper.GetService<IVolumeService>();
        private readonly IMediaControlService mediaService = ServiceHelper.GetService<IMediaControlService>();
        private readonly SleepTimerOrchestrator orchestrator = ServiceHelper.GetService<SleepTimerOrchestrator>();

        public SleepTimerService()
        {
            if (mainTimer == null)
                throw new NullReferenceException(nameof(mainTimer));


            var mediaController = new MediaController(this.volumeService, this.mediaService, appPreferences);
            var notifier = new TimerNotifier(appPreferences, (msg, level) => UpdateNotification(msg, level));

            mainPageDisplay.SetStartTime(appPreferences.TimerDurationSeconds);

            // Wire up events
            mainTimer.Tick += (s, remaining) =>
            {
                notifier.OnTick(remaining);
                mainPageDisplay.OnTick(remaining);
                if (remaining.TotalSeconds <= appPreferences.FadeOutSeconds)
                    mediaController.HandleFadeOut(remaining);
                else
                    mediaController.SetStartingVolume(); // refreshes every second in case user changed the volume. Stops updating only after the we are in the fade-out period.
            };
            //mainTimer.Finished += (s, e) => mediaController.StopPlayback();
            mainTimer.EnteredStandby += (s, e) => mediaController.EnterStandby();
            mainTimer.Finished += (s, e) =>
            {
                StopTimer();
                StopSelf();
                mediaController.HandleFinished();
            };
        }

        public override IBinder? OnBind(Intent? intent) => null;

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            if (intent?.Action == ServiceAction.Start.ToString())
            {
                var minutes = intent.GetIntExtra("minutes", appPreferences.TimerDurationSeconds);
#warning UpdateNotification uz mam jinde
                mainTimer.StartTimer(UpdateNotification);

                var notification = BuildNotification($"Starting timer. {appPreferences.TimerDurationSeconds} minutes left.");

                StartForeground(SERVICE_ID, notification);
            }
            else if (intent?.Action == ServiceAction.Extend.ToString())
            {
                mainTimer.Extend();
            }
            else if (intent?.Action == ServiceAction.Stop.ToString())
            {
                StopTimer();
                StopSelf();
            }

            return StartCommandResult.NotSticky;
        }
        private void StopTimer()
        {
            mainTimer.StopTimer();
        }

        private Notification BuildNotification(string message, NotificationLevel notificationLevel = NotificationLevel.High)
        {
#pragma warning disable CA1416, CA1422
            var channelId = "sleep_timer_channel";

            var builder = Build.VERSION.SdkInt >= BuildVersionCodes.O
                ? new Notification.Builder(this, channelId)
                : new Notification.Builder(this);

            builder.SetContentTitle("Sleep Timer")
                   .SetContentText($"{message} Tap to extend!")
                   .SetSmallIcon(global::Android.Resource.Drawable.IcLockIdleAlarm);

            // Handle priority for pre-26
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                builder.SetPriority((int)NotificationLevelHelper.MapToPriority(notificationLevel)); // silent but visible
                //System.Diagnostics.Debug.WriteLine("notification: " + MapToPriority(notificationLevel));
            }
            else
            {
                // Ensure the channel exists for API 26+
                var channel = new NotificationChannel(
                    channelId,
                    "Sleep Timer",
                    NotificationLevelHelper.MapToImportance(notificationLevel) //NotificationImportance.High
                );
                channel.SetSound(null, null);
                //System.Diagnostics.Debug.WriteLine("notification: " + MapToImportance(notificationLevel));
                var manager = (global::Android.App.NotificationManager?)base.GetSystemService(NotificationService)
                    ?? throw new InvalidOperationException("NotificationManager not available");
                manager.CreateNotificationChannel(channel);
            }

            var extendIntent = new Intent(this, typeof(SleepTimerService));
            extendIntent.SetAction(ServiceAction.Extend.ToString());
            var extendPending = PendingIntent.GetService(this, 1, extendIntent, PendingIntentFlags.Immutable);

            var stopIntent = new Intent(this, typeof(SleepTimerService));
            stopIntent.SetAction(ServiceAction.Stop.ToString());
            var stopPending = PendingIntent.GetService(this, 2, stopIntent, PendingIntentFlags.Immutable);

            builder
                .SetContentIntent(extendPending)
                .AddAction(new Notification.Action.Builder(0, ServiceAction.Stop.ToString(), stopPending).Build());

            var notification = builder.Build();

            return notification;
#pragma warning restore CA1416, CA1422
        }
        private void UpdateNotification(string remainingTime, NotificationLevel notificationLevel = NotificationLevel.High)
        {
            var notification = BuildNotification(remainingTime, notificationLevel);
            var manager = NotificationManagerCompat.From(this);
            manager.Notify(SERVICE_ID, notification);
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            StopTimer();

            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                StopForeground(StopForegroundFlags.Remove);
            }
            else
            {
#pragma warning disable CS0618 // Suppress obsolete warning for older versions
                StopForeground(true);
#pragma warning restore CS0618
            }

            var manager = NotificationManagerCompat.From(this);
            manager.Cancel(SERVICE_ID);
        }
    }
}
