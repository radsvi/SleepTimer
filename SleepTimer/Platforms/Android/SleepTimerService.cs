using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using System.Timers;

namespace SleepTimer.Platforms.Android
{
    public enum ServiceAction
    {
        Start,
        Postpone,
        Stop
    }
    [Service(ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeMediaPlayback)]
    public class SleepTimerService : Service, ISleepTimerService
    {
#warning smazat CancellationTokenSource? _cts
        private CancellationTokenSource? _cts;
        public System.Timers.Timer Timer { get; private set; } = new System.Timers.Timer();
        const int SERVICE_ID = 1001;
        private readonly AudioManager audioManager = (AudioManager?)global::Android.App.Application.Context.GetSystemService(Context.AudioService)
            ?? throw new InvalidOperationException("AudioService not available");
        private readonly AppPreferences appPreferences = ServiceHelper.GetService<AppPreferences>();
        private readonly MainTimer timerLogic = ServiceHelper.GetService<MainTimer>();

        public override IBinder? OnBind(Intent? intent) => null;

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            if (intent?.Action == ServiceAction.Start.ToString())
            {
                var minutes = intent.GetIntExtra("minutes", appPreferences.DefaultDuration);
#warning Revert back to "TimeSpan.FromMinutes(minutes)"
                StartTimer(TimeSpan.FromSeconds(5));
                //StartTimer(TimeSpan.FromMinutes(minutes));
            }
            else if (intent?.Action == ServiceAction.Postpone.ToString())
            {
                var logic = ServiceHelper.GetService<ISleepTimerLogic>();
                _ = logic.OnPostponeAsync();

                // restart timer
                //StartTimer(TimeSpan.FromMinutes(10));
            }
            else if (intent?.Action == ServiceAction.Stop.ToString())
            {
                StopTimer();
                StopSelf();
            }

            ShowNotification();
            return StartCommandResult.Sticky;
        }
        //private void StartTimer(TimeSpan duration)
        //{
        //    StopTimer(); // reset
        //    _cts = new CancellationTokenSource();

        //    Task.Run(async () =>
        //    {
        //        try
        //        {
        //            await Task.Delay(duration, _cts.Token);
        //            if (!_cts.IsCancellationRequested)
        //            {
        //                LowerMusicVolume();

        //                var logic = ServiceHelper.GetService<ISleepTimerLogic>();
        //                await logic.OnTimerElapsedAsync();


        //                StopSelf();
        //            }
        //        }
        //        catch (TaskCanceledException) { }
        //    });
        //}
        private async void StartTimer(TimeSpan duration)
        {
            //Timer.Elapsed += OnTimedEvent;
            //Timer.Interval = 1000; // seconds

            //EndTime = DateTime.Now.AddMinutes(appPreferences.DefaultDuration);
            //StartingVolume = volumeService.GetVolume();
            //Timer.Enabled = true;

            //if (EndTime != null)
            //    RemainingTime = (DateTime)EndTime - DateTime.Now;

            //LastNotificationUpdate = RemainingTime.Minutes;
            //await Notifications.Show(new NotificationMessageRemainingTime(RemainingTime.Minutes));

            //await timerLogic.OnTimedEvent();

            await timerLogic.Start();
        }
        //private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        //{
        //    await timerLogic.OnTimedEvent();
        //}
        public async Task OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            
        }

        private void StopTimer()
        {
            timerLogic.Stop();
        }

        private void ShowNotification()
        {
#pragma warning disable CA1416, CA1422
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
                    NotificationImportance.Low
                );

                var manager = (NotificationManager?)GetSystemService(NotificationService)
                    ?? throw new InvalidOperationException("NotificationManager not available");
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

            StartForeground(1, notification);
#pragma warning restore CA1416, CA1422
        }

        public override void OnDestroy()
        {
            StopTimer();
            base.OnDestroy();
        }

        //void LowerMusicVolume()
        //{
        //    int targetVolume = 0;
        //    int i = 0;

        //    while (GetVolume() > targetVolume)
        //    {
        //        // Simulate user volume button presses
        //        audioManager.AdjustStreamVolume(global::Android.Media.Stream.Music, Adjust.Lower, VolumeNotificationFlags.ShowUi);
        //        //audioManager.AdjustStreamVolume(global::Android.Media.Stream.Music, Adjust.Lower, 0); // hide UI

        //        Task.Delay(200).Wait();
        //        i++;
        //        if (i >= 100)
        //            throw new InvalidOperationException("Couldn't lower the volume.");
        //    }
        //}
        //int GetVolume()
        //{
        //    return this.audioManager.GetStreamVolume(global::Android.Media.Stream.Music);
        //}
    }
}
