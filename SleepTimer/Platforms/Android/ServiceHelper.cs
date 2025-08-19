using Android.Content;

namespace SleepTimer.Platforms.Android
{
    public static class ServiceHelper
    {
        public static void StartVolumeService()
        {
            var context = global::Android.App.Application.Context;
            var intent = new Intent(context, typeof(VolumeForegroundService));
            context.StartForegroundService(intent);
        }
        public static void StopVolumeService()
        {
            var context = global::Android.App.Application.Context;
            var intent = new Intent(context, typeof(VolumeForegroundService));
            context.StopService(intent);
        }
    }
}
