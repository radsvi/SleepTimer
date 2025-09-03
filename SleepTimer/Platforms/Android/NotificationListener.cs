using Android.App;
using Android.Service.Notification;

namespace SleepTimer.Platforms.Android
{
    [Service(Label = "SleepTimer Notification Listener",
         Permission = "android.permission.BIND_NOTIFICATION_LISTENER_SERVICE",
         Exported = true)]
    [IntentFilter(new[] { "android.service.notification.NotificationListenerService" })]
    public class NotificationListener : NotificationListenerService {}
}
