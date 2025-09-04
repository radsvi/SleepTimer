using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using AndroidX.Core.App;


namespace SleepTimer.Platforms.Android.Services
{
    [Service(ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeMediaPlayback)]
    public class SleepTimerService : Service, ISleepTimerService
    {
        const int SERVICE_ID = 1001;
        private readonly AudioManager audioManager = (AudioManager?)global::Android.App.Application.Context.GetSystemService(AudioService)
            ?? throw new InvalidOperationException("AudioService not available");
        private readonly AppPreferences appPreferences = ServiceHelper.GetService<AppPreferences>();
        private readonly MainTimer timerLogic = ServiceHelper.GetService<MainTimer>();

        public override IBinder? OnBind(Intent? intent) => null;

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            if (intent?.Action == ServiceAction.Start.ToString())
            {
                var minutes = intent.GetIntExtra("minutes", appPreferences.DefaultDuration);
                timerLogic.StartTimer(UpdateNotification);

                var notification = BuildNotification($"Starting timer. {appPreferences.DefaultDuration} minutes left.");
                StartForeground(SERVICE_ID, notification);
            }
            else if (intent?.Action == ServiceAction.Extend.ToString())
            {
                timerLogic.Extend();
            }
            else if (intent?.Action == ServiceAction.Stop.ToString())
            {
                StopTimer();
                StopSelf();
            }

            return StartCommandResult.Sticky;
        }
        private void StopTimer()
        {
            timerLogic.StopTimer();
        }

        private Notification BuildNotification(string message)
        {
#pragma warning disable CA1416, CA1422
            var channelId = "sleep_timer_channel";

            var builder = Build.VERSION.SdkInt >= BuildVersionCodes.O
                ? new Notification.Builder(this, channelId)
                : new Notification.Builder(this);

            builder.SetContentTitle("Sleep Timer")
                   .SetContentText($"{message} Tap to extend!")
                   .SetSmallIcon(global::Android.Resource.Drawable.IcMediaPlay);

            // Handle priority for pre-26
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                builder.SetPriority((int)NotificationPriority.Low); // silent but visible
            }
            else
            {
                // Ensure the channel exists for API 26+
                var channel = new NotificationChannel(
                    channelId,
                    "Sleep Timer",
                    NotificationImportance.Low
                );

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
                .AddAction(new Notification.Action.Builder(0, ServiceAction.Stop.ToString(), stopPending).Build());

            var notification = builder.Build();

            return notification;
#pragma warning restore CA1416, CA1422
        }
        private void UpdateNotification(string remainingTime)
        {
            var notification = BuildNotification(remainingTime);
            var manager = NotificationManagerCompat.From(this);
            manager.Notify(1001, notification);
        }
        public override void OnDestroy()
        {
            StopTimer();
            base.OnDestroy();
        }
    }
}
