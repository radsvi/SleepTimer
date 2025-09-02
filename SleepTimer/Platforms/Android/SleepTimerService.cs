using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;

namespace SleepTimer.Platforms.Android
{
    public enum ServiceAction
    {
        Start,
        Postpone,
        Stop
    }
    [Service(ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeMediaPlayback)]
    public class SleepTimerService : Service
    {
        private CancellationTokenSource? _cts;

        public override IBinder? OnBind(Intent? intent) => null;

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            if (intent?.Action == ServiceAction.Start.ToString())
            {
                var minutes = intent.GetIntExtra("minutes", 5);
                StartTimer(TimeSpan.FromMinutes(minutes));
            }
            else if (intent?.Action == ServiceAction.Postpone.ToString())
            {
                var logic = ServiceHelper.GetService<ISleepTimerLogic>();
                _ = logic.OnPostponeAsync();

                // restart timer
                StartTimer(TimeSpan.FromMinutes(10));
            }
            else if (intent?.Action == ServiceAction.Stop.ToString())
            {
                StopTimer();
                StopSelf();
            }

            ShowNotification();
            return StartCommandResult.Sticky;
        }

        private void StartTimer(TimeSpan duration)
        {
            StopTimer(); // reset
            _cts = new CancellationTokenSource();

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(duration, _cts.Token);
                    if (!_cts.IsCancellationRequested)
                    {
                        var logic = ServiceHelper.GetService<ISleepTimerLogic>();
                        await logic.OnTimerElapsedAsync();
                        StopSelf();
                    }
                }
                catch (TaskCanceledException) { }
            });
        }

        private void StopTimer()
        {
            _cts?.Cancel();
            _cts = null;
        }

        private void ShowNotification()
        {
            var channelId = "sleep_timer_channel";

            var builder = Build.VERSION.SdkInt >= BuildVersionCodes.O
                ? new Notification.Builder(this, channelId)
                : new Notification.Builder(this);

            builder.SetContentTitle("Sleep Timer")
                   .SetContentText("Timer is running")
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
                    NotificationImportance.Low // silent but visible
                );

                var manager = (NotificationManager)GetSystemService(NotificationService);
                manager.CreateNotificationChannel(channel);
            }

            var postponeIntent = new Intent(this, typeof(SleepTimerService));
            postponeIntent.SetAction(ServiceAction.Postpone.ToString());
            var postponePending = PendingIntent.GetService(this, 1, postponeIntent, PendingIntentFlags.Immutable);

            var stopIntent = new Intent(this, typeof(SleepTimerService));
            stopIntent.SetAction(ServiceAction.Stop.ToString());
            var stopPending = PendingIntent.GetService(this, 2, stopIntent, PendingIntentFlags.Immutable);

            builder.AddAction(new Notification.Action.Builder(0, ServiceAction.Postpone.ToString(), postponePending).Build())
                .AddAction(new Notification.Action.Builder(0, ServiceAction.Stop.ToString(), stopPending).Build());

            var notification = builder.Build();

            //if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            //{
            //    var channel = new NotificationChannel(channelId, "Sleep Timer", NotificationImportance.Low);
            //    var manager = (NotificationManager)GetSystemService(NotificationService);
            //    manager.CreateNotificationChannel(channel);
            //}

            //var postponeIntent = new Intent(this, typeof(SleepTimerService));
            //postponeIntent.SetAction("POSTPONE");
            //var postponePending = PendingIntent.GetService(this, 1, postponeIntent, PendingIntentFlags.Immutable);

            //var stopIntent = new Intent(this, typeof(SleepTimerService));
            //stopIntent.SetAction("STOP");
            //var stopPending = PendingIntent.GetService(this, 2, stopIntent, PendingIntentFlags.Immutable);

            //var notification = new Notification.Builder(this, channelId)
            //    .SetContentTitle("Sleep Timer")
            //    .SetContentText("Timer is running")
            //    .SetSmallIcon(global::Android.Resource.Drawable.IcMediaPlay)
            //    .AddAction(new Notification.Action.Builder(0, "Postpone", postponePending).Build())
            //    .AddAction(new Notification.Action.Builder(0, "Stop", stopPending).Build())
            //    .Build();

            StartForeground(1, notification);
        }

        public override void OnDestroy()
        {
            StopTimer();
            base.OnDestroy();
        }
    }
}
