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

            
            if (!HasNotificationAccess())
            {
                RequestNotificationAccess();
                return; // Interrupting here. User has to grant access manually.
            }
            
            var mediaSessionManager = (MediaSessionManager)context.GetSystemService(Context.MediaSessionService);
            var componentName = new ComponentName(context, Java.Lang.Class.FromType(typeof(NotificationListener)).Name);
            var sessions = mediaSessionManager.GetActiveSessions(componentName);

            foreach (var controller in sessions)
            {
                try
                {
                    controller?.GetTransportControls()?.Pause();
                }
                catch {}
            }
        }
        private static bool HasNotificationAccess()
        {
            var context = global::Android.App.Application.Context;
            var enabledListeners = Settings.Secure.GetString(global::Android.App.Application.Context.ContentResolver, "enabled_notification_listeners");
            return !string.IsNullOrEmpty(enabledListeners) && enabledListeners.Contains(context.PackageName);
        }

        /// <summary>
        /// Sends user to Notification Access settings
        /// </summary>
        private static void RequestNotificationAccess()
        {
            var context = global::Android.App.Application.Context;
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
        public async void CheckNotificationAccess()
        {
            if (HasNotificationAccess())
                return;
            
            var answer = await App.Current!.Windows[0].Page!.DisplayAlert("Permissions", "Missing permission to send Stop broadcast.", "Open permission settings", "Close");

            if (answer)
                RequestNotificationAccess();
        }
    }
}
