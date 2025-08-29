using Android.App;
using Android.Content;

namespace SleepTimer.Platforms.Android
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { "MYAPP_NOTIFICATION_TAP" })]
    public class NotificationTapReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var volumeService = MauiApplication.Current.Services.GetService<IVolumeService>();
            string data = intent.GetStringExtra("payload");

            //var current = volumeService.GetVolume();
            //if (current >= 0)
            //    volumeService.SetVolume(current - 10);
            volumeService?.SetVolume(50);
        }
    }
}
