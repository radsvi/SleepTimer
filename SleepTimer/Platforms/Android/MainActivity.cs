using Android.App;
using Android.Content.PM;
using Android.OS;

using Android;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace SleepTimer
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        const int RequestStorageId = 1001;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SleepTimer.Platforms.Android.RequestPermissionHelper.RequestNotificationPermission();

            // Check storage permission
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this,
                    new string[] { Manifest.Permission.WriteExternalStorage },
                    RequestStorageId);
            }
        }
        protected override void OnResume()
        {
            base.OnResume();

            SleepTimer.Platforms.Android.RequestPermissionHelper.RequestNotificationPermission();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestStorageId)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    System.Diagnostics.Debug.WriteLine("✅ Storage permission granted!");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("❌ Storage permission denied!");
                }
            }
        }
    }
}