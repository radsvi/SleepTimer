using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using System.Threading.Tasks;

namespace SleepTimer.Platforms.Android
{
    [Service(Enabled = true, ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeMediaPlayback)]
    public class VolumeAdjustService : Service
    {
        const int SERVICE_ID = 1001;
        private readonly AudioManager audioManager = (AudioManager)global::Android.App.Application.Context.GetSystemService(Context.AudioService)!;
        public override IBinder OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            StartForegroundServiceNotification();

            Task.Run(async () =>
            {
                await Task.Delay(5000);

                LowerMusicVolume();

                StopSelf();
            });

            return StartCommandResult.NotSticky;
        }

        void StartForegroundServiceNotification()
        {
            var channelId = "volume_service_channel";

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(channelId, "Volume Service", NotificationImportance.Low);
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }

            var notification = new Notification.Builder(this, channelId)
                .SetContentTitle("Volume Service")
                .SetContentText("Adjusting volume in background")
                .SetSmallIcon(global::Android.Resource.Drawable.IcMediaPlay)
                .Build();

            StartForeground(SERVICE_ID, notification);
        }

        void LowerMusicVolume()
        {
            int targetVolume = 0;

            while (GetVolume() > targetVolume)
            {
                // Simulate user volume button presses
                audioManager.AdjustStreamVolume(global::Android.Media.Stream.Music, Adjust.Lower, VolumeNotificationFlags.ShowUi);
                //audioManager.AdjustStreamVolume(global::Android.Media.Stream.Music, Adjust.Lower, 0); // hide UI

                Task.Delay(200).Wait();
            }
        }
        int GetVolume()
        {
            return this.audioManager.GetStreamVolume(global::Android.Media.Stream.Music);
        }
    }
}
