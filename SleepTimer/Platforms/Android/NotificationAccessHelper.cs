using Android.App;
using Android.Content;
using Android.Provider;
using Android.Service.Notification;

namespace SleepTimer.Platforms.Android
{
    public static class NotificationAccessHelper
    {
        public static bool HasNotificationAccess(Context context)
        {
            var enabledListeners = Settings.Secure.GetString(context.ContentResolver, "enabled_notification_listeners");
            return enabledListeners != null && enabledListeners.Contains(context.PackageName);
        }

        public static void RequestNotificationAccess(Context context)
        {
            Intent intent = new Intent("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS");
            intent.AddFlags(ActivityFlags.NewTask);
            context.StartActivity(intent);
        }
    }
}
