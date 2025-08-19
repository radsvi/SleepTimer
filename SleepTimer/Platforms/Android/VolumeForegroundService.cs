using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;

namespace SleepTimer.Platforms.Android
{
    [Service(ForegroundServiceType = ForegroundService.TypeMediaPlayback)]
    public class VolumeForegroundService : Service
    {
        public static VolumeForegroundService? Instance;
        private AudioManager? _audioManager;

        public override void OnCreate()
        {
            base.OnCreate();
            Instance = this;
            _audioManager = (AudioManager)global::Android.App.Application.Context.GetSystemService(Context.AudioService)!;
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            Instance = null; // clear when service is stopped
        }

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            // Start as Foreground Service
            StartForeground(1, CreateNotification());
            return StartCommandResult.Sticky;
        }

        public override IBinder? OnBind(Intent? intent) => null;

        private Notification CreateNotification()
        {
            var channelId = "volume_service_channel";
            var channelName = "Volume Service";

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(channelId, channelName, NotificationImportance.Low);
                var manager = (NotificationManager)global::Android.App.Application.Context.GetSystemService(Context.NotificationService)!;
                manager.CreateNotificationChannel(channel);
            }

            var builder = new Notification.Builder(this, channelId)
                .SetContentTitle("Volume Service Running")
                .SetContentText("Managing volume in background")
                .SetSmallIcon(global::Android.Resource.Drawable.IcMediaPlay);

            return builder.Build();
        }

        // Custom method to change volume
        public void SetVolume(int level)
        {
            if (_audioManager == null) return;
            int maxVolume = _audioManager.GetStreamMaxVolume(global::Android.Media.Stream.Music);
            int newVolume = (int)(Math.Clamp(level, 0, 100) / 100.0 * maxVolume);
            _audioManager.SetStreamVolume(global::Android.Media.Stream.Music, newVolume, 0);
        }
    }
}
