using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using System.Threading.Tasks;

namespace SleepTimer.Platforms.Android
{
    [Service(Enabled = true)]
    public class VolumeAdjustService : Service
    {
        const int SERVICE_ID = 1001;

        public override IBinder OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            StartForegroundServiceNotification();

            // Run the timer in a background task
            Task.Run(async () =>
            {
                await Task.Delay(5000); // wait 1 minute

                LowerMusicVolume();

                StopSelf(); // stop the service after volume adjustment
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
            var audioManager = (AudioManager)global::Android.App.Application.Context.GetSystemService(Context.AudioService);

            int currentVolume = audioManager.GetStreamVolume(global::Android.Media.Stream.Music);

            // Example: lower volume by 3 steps (you can adjust)
            int targetVolume = 5;

            // Simulate user volume button presses
            while (currentVolume > targetVolume)
            {
                audioManager.AdjustStreamVolume(global::Android.Media.Stream.Music, Adjust.Lower, VolumeNotificationFlags.ShowUi);
                currentVolume--;
                Task.Delay(200).Wait(); // small delay between steps
            }
        }
    }
}
