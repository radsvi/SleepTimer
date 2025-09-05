﻿using Android.App;
using Android.Content.PM;
using Android.OS;

namespace SleepTimer
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SleepTimer.Platforms.Android.RequestPermissionHelper.RequestNotificationPermission();
        }
        protected override void OnResume()
        {
            base.OnResume();

            SleepTimer.Platforms.Android.RequestPermissionHelper.RequestNotificationPermission();
        }
    }
}