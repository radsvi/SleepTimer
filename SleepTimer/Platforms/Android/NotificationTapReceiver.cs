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
            // Handle notification tap WITHOUT launching app
            string data = intent.GetStringExtra("payload");
            //System.Diagnostics.Debug.WriteLine($"Notification tapped with payload: {data}");

            
        }
    }
}
