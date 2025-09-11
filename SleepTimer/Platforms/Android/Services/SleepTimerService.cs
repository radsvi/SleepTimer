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
        //private readonly AudioManager audioManager = (AudioManager?)global::Android.App.Application.Context.GetSystemService(AudioService) ?? throw new NullReferenceException(nameof(audioManager));
        private readonly SleepTimerOrchestrator orchestrator;

        public SleepTimerService()
        {
            ////var notifier = new TimerNotifier(appPreferences, (msg, level) => UpdateNotification(msg, level));

            //var notificationManager = new NotificationManagerWrapper(this);
            ////orchestrator = new SleepTimerOrchestrator(mainTimer, appPreferences, volumeService, mediaService, mainPageDisplay, notificationManager);

            //orchestrator = ServiceHelper.GetService<SleepTimerOrchestrator>();




            var mainTimer = ServiceHelper.GetService<MainTimer>();
            var appPreferences = ServiceHelper.GetService<AppPreferences>();
            //var volumeService = ServiceHelper.GetService<IVolumeService>();
            //var mediaService = ServiceHelper.GetService<IMediaControlService>();
            var mediaController = ServiceHelper.GetService<MediaController>();
            var mainPageDisplay = ServiceHelper.GetService<MainPageDisplay>();
            
            var notificationManager = new NotificationManagerWrapper(this);
            orchestrator = new SleepTimerOrchestrator(mainTimer, appPreferences, mediaController, mainPageDisplay, notificationManager);





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
            manager?.Cancel(Constants.SERVICE_ID);
        }
    }
}
