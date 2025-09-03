using Android.App;
using Android.Content;
using Android.Media.Session;
using Android.Service.Notification;
using Android.Provider;
using Android.OS;
using SleepTimer.Platforms.Android.Services;

namespace SleepTimer.Platforms.Android
{
    public class MediaControlService : IMediaControlService
    {
        public void StopPlayback()
        {
            var context = global::Android.App.Application.Context;

            // 1️⃣ Check notification access
            if (!HasNotificationAccess(context))
            {
                RequestNotificationAccess(context);
                return; // Wait for user to grant access
            }

            // 2️⃣ Access MediaSessionManager and pause sessions
            var mediaSessionManager = (MediaSessionManager)context.GetSystemService(Context.MediaSessionService);
            var componentName = new ComponentName(context, Java.Lang.Class.FromType(typeof(NotificationListener)).Name);
            var sessions = mediaSessionManager.GetActiveSessions(componentName);

            foreach (var controller in sessions)
            {
                try
                {
                    controller?.GetTransportControls()?.Pause();
                }
                catch
                {
                    // Some sessions may be non-controllable
                }
            }
        }
        private bool HasNotificationAccess(Context context)
        {
            var enabledListeners = Settings.Secure.GetString(context.ContentResolver, "enabled_notification_listeners");
            return !string.IsNullOrEmpty(enabledListeners) && enabledListeners.Contains(context.PackageName);
        }

        // Send user to Notification Access settings
        private void RequestNotificationAccess(Context context)
        {
            Intent intent;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                // Android 11+
                intent = new Intent(Settings.ActionNotificationListenerSettings);
            }
            else
            {
                // Older versions
                intent = new Intent("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS");
            }
            intent.AddFlags(ActivityFlags.NewTask);
            context.StartActivity(intent);
        }
    }
}
