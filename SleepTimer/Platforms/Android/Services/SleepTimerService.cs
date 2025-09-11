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
        private readonly AppPreferences appPreferences;
        private readonly MainTimer mainTimer;
        private readonly MainPageDisplay mainPageDisplay;
        private readonly IVolumeService volumeService;
        private readonly IMediaControlService mediaService;
        private readonly SleepTimerOrchestrator orchestrator;
        private readonly NotificationManager notificationManager;

        public SleepTimerService()
        {
            //var notifier = new TimerNotifier(appPreferences, (msg, level) => UpdateNotification(msg, level));

            mainTimer = ServiceHelper.GetService<MainTimer>();
            appPreferences = ServiceHelper.GetService<AppPreferences>();
            volumeService = ServiceHelper.GetService<IVolumeService>();
            mediaService = ServiceHelper.GetService<IMediaControlService>();
            mainPageDisplay = ServiceHelper.GetService<MainPageDisplay>();
            notificationManager = ServiceHelper.GetService<NotificationManager>();
            orchestrator = ServiceHelper.GetService<SleepTimerOrchestrator>();


            orchestrator.TimerStoppedOrFinished += (s, e) =>
            {
                StopSelf(); // Service lifecycle decision stays here
            };
        }

        public override IBinder? OnBind(Intent? intent) => null;

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            orchestrator.HandleIntent(intent);

            return StartCommandResult.NotSticky;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

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
            manager?.Cancel(SERVICE_ID);
        }
    }
}
