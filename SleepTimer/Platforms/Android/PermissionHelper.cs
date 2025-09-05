using Android.App;
using Android.Content;
using Android.Media.Session;
using Android.Service.Notification;
using Android.Provider;
using Android.OS;
using SleepTimer.Platforms.Android.Services;
using Android;
using Android.Content.PM;
using AndroidX.Core.Content;

namespace SleepTimer.Platforms.Android
{
    public class PermissionHelper : IPermissionHelper
    {
        private readonly Context context;
        public PermissionHelper()
        {
            context = global::Android.App.Application.Context;
        }
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







        private bool HasNotificationAccess()
        {
            if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.Tiramisu)
            {
                // On Android 13+ check the POST_NOTIFICATIONS permission
                return ContextCompat.CheckSelfPermission(context, Manifest.Permission.PostNotifications)
                       == Permission.Granted;
            }

            // On Android < 13 notifications are enabled by default
            return true;
        }


        private void RequestNotificationAccess()
        {
            Intent intent = new Intent();
            if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O)
            {
                // Android 8+ - open app-specific notification settings
                intent.SetAction(global::Android.Provider.Settings.ActionAppNotificationSettings);
                intent.PutExtra(global::Android.Provider.Settings.ExtraAppPackage, context.PackageName);
            }
            else
            {
                // Older versions - open the app's settings page
                intent.SetAction(global::Android.Provider.Settings.ActionApplicationDetailsSettings);
                intent.SetData(global::Android.Net.Uri.Parse("package:" + context.PackageName));
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
