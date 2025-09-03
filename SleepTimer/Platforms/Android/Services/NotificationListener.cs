using Android.App;
using Android.Service.Notification;

namespace SleepTimer.Platforms.Android.Services
{
    [Service(Name = "com.companyname.sleeptimer.NotificationListener",
        Label = "SleepTimer Notification Listener",
        Permission = "android.permission.BIND_NOTIFICATION_LISTENER_SERVICE",
        Exported = true)]
    [IntentFilter(new[] { "android.service.notification.NotificationListenerService" })]
    public class NotificationListener : NotificationListenerService {}
}