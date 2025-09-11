using Android.App;
using Android.Content;
using AndroidX.Core.App;

namespace SleepTimer.Platforms.Android
{
    public static class ServiceHelper
    {
        //public static T GetService<T>()
        //{
        //    return MauiApplication.Current.Services.GetRequiredService<T>();
        //}
        public static T GetService<T>([CallerMemberName] string? propertyName = null)
        {
            //var service = DependencyService.Get<IRequestPermissionHelper>();
            //service?.RequestNotificationPermission();

            var result = Current.GetService<T>() ?? throw new NullReferenceException(nameof(propertyName));
            return result;
        }

        public static IServiceProvider Current =>
            IPlatformApplication.Current?.Services
            ?? throw new NullReferenceException("No service provider.");
    }
}
