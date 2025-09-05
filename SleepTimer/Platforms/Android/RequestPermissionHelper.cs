using Android;
using Android.Content.PM;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace SleepTimer.Platforms.Android
{
    public class RequestPermissionHelper
    {
        public static void RequestNotificationPermission(global::Android.App.Activity activity)
        {
            int RequestCode = 1001;
            if (ContextCompat.CheckSelfPermission(activity, Manifest.Permission.PostNotifications)
                != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(activity,
                    new string[] { Manifest.Permission.PostNotifications }, RequestCode);
            }
        }
    }
}
