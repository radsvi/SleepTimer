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
        private readonly AudioManager audioManager = (AudioManager?)global::Android.App.Application.Context.GetSystemService(AudioService)
            ?? throw new InvalidOperationException("AudioService not available");
        private readonly AppPreferences appPreferences = ServiceHelper.GetService<AppPreferences>() ?? throw new NullReferenceException(nameof(appPreferences));
        private readonly MainTimer mainTimer = ServiceHelper.GetService<MainTimer>() ?? throw new NullReferenceException(nameof(mainTimer));
        private readonly MainPageDisplay mainPageDisplay = ServiceHelper.GetService<MainPageDisplay>() ?? throw new NullReferenceException(nameof(mainPageDisplay));
        private readonly IVolumeService volumeService = ServiceHelper.GetService<IVolumeService>() ?? throw new NullReferenceException(nameof(volumeService));
        private readonly IMediaControlService mediaService = ServiceHelper.GetService<IMediaControlService>() ?? throw new NullReferenceException(nameof(mediaService));

        public SleepTimerService()
        {
            //OnTimeFinished += TimeFinished;

            if (mainTimer == null)
                throw new NullReferenceException(nameof(mainTimer));


            var mediaController = new MediaController(this.volumeService, this.mediaService, appPreferences);
            //var notifier = new SleepTimerNotifier(appPreferences, (msg, level) => Debug.WriteLine($"{level}: {msg}"));
            var notifier = new TimerNotifier(appPreferences, (msg, level) => UpdateNotification(msg, level));
                //=> Debug.WriteLine($"{level}: {msg}"));
            //var notification = BuildNotification($"Starting timer. {appPreferences.DefaultDuration} minutes left.");

            mainPageDisplay.SetStartTime(appPreferences.DefaultDuration);

            // Wire up events
            mainTimer.Tick += (s, remaining) =>
            {
                notifier.OnTick(remaining);
                mainPageDisplay.OnTick(remaining);
                if (remaining.TotalSeconds <= appPreferences.FadeOutDuration)
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

        //public event EventHandler OnTimeFinished;

        public override IBinder? OnBind(Intent? intent) => null;

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            if (intent?.Action == ServiceAction.Start.ToString())
            {
                var minutes = intent.GetIntExtra("minutes", appPreferences.DefaultDuration);
#warning UpdateNotification uz mam jinde
                mainTimer.StartTimer(UpdateNotification);

                var notification = BuildNotification($"Starting timer. {appPreferences.DefaultDuration} minutes left.");

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
#pragma warning disable CA1416
        private static NotificationImportance MapToImportance(NotificationLevel level) => level switch
        {
            NotificationLevel.Min => NotificationImportance.Min,
            NotificationLevel.Low => NotificationImportance.Low,
            NotificationLevel.Default => NotificationImportance.Default,
            NotificationLevel.High => NotificationImportance.High,
            NotificationLevel.Max => NotificationImportance.High, // no direct Max
            _ => NotificationImportance.Default
        };
#pragma warning restore CA1416

        private static int MapToPriority(NotificationLevel level) => level switch
        {
            NotificationLevel.Min => (int)NotificationPriority.Min,
            NotificationLevel.Low => (int)NotificationPriority.Low,
            NotificationLevel.Default => (int)NotificationPriority.Default,
            NotificationLevel.High => (int)NotificationPriority.High,
            NotificationLevel.Max => (int)NotificationPriority.Max,
            _ => (int)NotificationPriority.Default
        };

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
                builder.SetPriority((int)MapToPriority(notificationLevel)); // silent but visible
                //System.Diagnostics.Debug.WriteLine("notification: " + MapToPriority(notificationLevel));
            }
            else
            {
                // Ensure the channel exists for API 26+
                var channel = new NotificationChannel(
                    channelId,
                    "Sleep Timer",
                    MapToImportance(notificationLevel) //NotificationImportance.High
                );
                channel.SetSound(null, null);
                //System.Diagnostics.Debug.WriteLine("notification: " + MapToImportance(notificationLevel));
                var manager = (NotificationManager?)GetSystemService(NotificationService)
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
                //.AddAction(new Notification.Action.Builder(0, ServiceAction.Extend.ToString(), postponePending).Build())
                //.AddAction(Resource.Drawable.sleepzz, "Press Me", stopPending)
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
