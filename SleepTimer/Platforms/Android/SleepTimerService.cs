using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using AndroidX.Core.App;


namespace SleepTimer.Platforms.Android
{
    public enum ServiceAction
    {
        Start,
        Extend,
        Stop,
        StopPlayback
    }
    [Service(ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeMediaPlayback)]
    public class SleepTimerService : Service, ISleepTimerService
    {
        //private CancellationTokenSource? _cts;
        //public System.Timers.Timer Timer { get; private set; } = new System.Timers.Timer();
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
                StartTimer(TimeSpan.FromMinutes(minutes));

                var notification = BuildNotification("Starting timer.");
                StartForeground(SERVICE_ID, notification);
            }
            else if (intent?.Action == ServiceAction.StopPlayback.ToString())
            {
                //var mediaControlService = ServiceHelper.GetService<IMediaControlService>();
                //mediaControlService.StopPlayback();
                //var mediaControlService = ServiceHelper.GetService<IMediaControlService>()
                //    ?? throw new InvalidOperationException("No service provider.");
                //mediaControlService.StopPlayback();

                //RequestFocus();

                //var mediaControlService = ServiceHelper.GetService<MediaPlaybackBroadcast>();
                var mediaPlaybackBroadcast = ServiceHelper.GetService<IMediaPlaybackBroadcast>()
                    ?? throw new InvalidOperationException("No service provider.");
                mediaPlaybackBroadcast.SendMediaPause();


            }
            else if (intent?.Action == ServiceAction.Extend.ToString())
            {
                var logic = ServiceHelper.GetService<ISleepTimerLogic>();
                _ = logic.OnPostponeAsync();

                Extend();
            }
            else if (intent?.Action == ServiceAction.Stop.ToString())
            {
                StopTimer();
                StopSelf();
            }

            return StartCommandResult.Sticky;
        }

        //async void RequestFocus()
        //{
        //    var audioFocusHelper = ServiceHelper.GetService<IAudioFocusHelper>();

        //    var audioPlayer = new SilentAudioPlayer();
        //    audioPlayer.Start();

        //    audioFocusHelper.RequestAudioFocus();

        //    await Task.Delay(15000);
        //    audioPlayer.Stop();
        //    audioFocusHelper.AbandonAudioFocus();
        //}
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
        private void StartTimer(TimeSpan duration)
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

            timerLogic.Start(UpdateNotification);
        }
        //private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        //{
        //    await timerLogic.OnTimedEvent();
        //}
        private void Extend()
        {
            timerLogic.Extend();
        }


        private void StopTimer()
        {
            timerLogic.Stop();
        }

        private Notification BuildNotification(string remainingTime)
        {
#pragma warning disable CA1416, CA1422
            var channelId = "sleep_timer_channel";

            var builder = Build.VERSION.SdkInt >= BuildVersionCodes.O
                ? new Notification.Builder(this, channelId)
                : new Notification.Builder(this);

            builder.SetContentTitle("Sleep Timer")
                   .SetContentText($"{remainingTime} Tap to extend!")
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

            var stopPlaybackIntent = new Intent(this, typeof(SleepTimerService));
            stopPlaybackIntent.SetAction(ServiceAction.StopPlayback.ToString());
            var stopPlaybackPending = PendingIntent.GetService(this, 3, stopPlaybackIntent, PendingIntentFlags.Immutable);

            var stopIntent = new Intent(this, typeof(SleepTimerService));
            stopIntent.SetAction(ServiceAction.Stop.ToString());
            var stopPending = PendingIntent.GetService(this, 2, stopIntent, PendingIntentFlags.Immutable);

            builder
                .SetContentIntent(extendPending)
                //.AddAction(new Notification.Action.Builder(0, ServiceAction.Extend.ToString(), postponePending).Build())
                .AddAction(new Notification.Action.Builder(0, ServiceAction.StopPlayback.ToString(), stopPlaybackPending).Build())
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
