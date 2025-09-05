using Android.Content;
using Android.OS;
using Android.Provider;


namespace SleepTimer.Platforms.Android
{
    public class PermissionHelper : IPermissionHelper
    {
        private readonly Context context = global::Android.App.Application.Context;
        private bool HasNotificationListenerAccess()
        {
            var enabledListeners = Settings.Secure.GetString(global::Android.App.Application.Context.ContentResolver, "enabled_notification_listeners");
            return !string.IsNullOrEmpty(enabledListeners) && enabledListeners.Contains(context.PackageName);
        }
        /// <summary>
        /// Sends user to Notification Access settings
        /// </summary>
        private void RequestNotificationListenerAccess()
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


        public async void CheckPermissions()
        {
            if (HasNotificationListenerAccess())
                return;

            var answer = await App.Current!.Windows[0].Page!.DisplayAlert("Permissions", "Missing permission to send Stop broadcast.", "Open permission settings", "Close");

            if (answer)
                RequestNotificationListenerAccess();
        }
    }
}
