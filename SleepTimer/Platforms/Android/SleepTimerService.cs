using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;

namespace SleepTimer.Platforms.Android
{
    [Service(ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeMediaPlayback)]
    public class SleepTimerService : Service
    {
        private CancellationTokenSource? _cts;

        public override IBinder? OnBind(Intent? intent) => null;

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            if (intent?.Action == "START")
            {
                var minutes = intent.GetIntExtra("minutes", 5);
                StartTimer(TimeSpan.FromMinutes(minutes));
            }
            else if (intent?.Action == "POSTPONE")
            {
                var logic = ServiceHelper.GetService<ISleepTimerLogic>();
                _ = logic.OnPostponeAsync();

                // restart timer
                StartTimer(TimeSpan.FromMinutes(10));
            }
            else if (intent?.Action == "STOP")
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

            // Ensure channel exists
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(channelId, "Sleep Timer", NotificationImportance.Default);
                var manager = (NotificationManager)GetSystemService(NotificationService);
                manager.CreateNotificationChannel(channel);
            }

            var postponeIntent = new Intent(this, typeof(SleepTimerService));
            postponeIntent.SetAction("POSTPONE");
            var postponePending = PendingIntent.GetService(this, 1, postponeIntent, PendingIntentFlags.Immutable);

            var stopIntent = new Intent(this, typeof(SleepTimerService));
            stopIntent.SetAction("STOP");
            var stopPending = PendingIntent.GetService(this, 2, stopIntent, PendingIntentFlags.Immutable);

            var notification = new Notification.Builder(this, channelId)
                .SetContentTitle("Sleep Timer")
                .SetContentText("Timer is running")
                .SetSmallIcon(global::Android.Resource.Drawable.IcMediaPlay)
                .AddAction(new Notification.Action.Builder(0, "Postpone", postponePending).Build())
                .AddAction(new Notification.Action.Builder(0, "Stop", stopPending).Build())
                .Build();

            StartForeground(1, notification);
        }

        public override void OnDestroy()
        {
            StopTimer();
            base.OnDestroy();
        }
    }
}
