using Android.App;
using Android.Content;
using AndroidX.Core.App;

namespace SleepTimer.Platforms.Android
{
    //public static class ServiceHelper
    //{
    //    //public static void StartVolumeService()
    //    //{
    //    //    var context = global::Android.App.Application.Context;
    //    //    var intent = new Intent(context, typeof(VolumeForegroundService));
    //    //    context.StartForegroundService(intent);
    //    //}
    //    public static void StartVolumeService()
    //    {
    //        var context = global::Android.App.Application.Context;
    //        var channelId = "silent_channel";
    //        var intent = new Intent("MYAPP_NOTIFICATION_TAP");
    //        intent.PutExtra("payload", "some data");
    //        var pendingIntent = PendingIntent.GetBroadcast(
    //            context,
    //            0,
    //            intent,
    //            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

    //        var builder = new NotificationCompat.Builder(context, channelId)
    //            .SetContentTitle("My Notification")
    //            .SetContentText("Tap me, but I won't open the app")
    //            .SetSmallIcon(global::Android.Resource.Drawable.IcMediaPlay)
    //            .SetContentIntent(pendingIntent) // Goes to BroadcastReceiver
    //            .SetAutoCancel(true);

    //        NotificationManagerCompat.From(context).Notify(1001, builder.Build());
    //    }
    //    public static void StopVolumeService()
    //    {
    //        var context = global::Android.App.Application.Context;
    //        var intent = new Intent(context, typeof(VolumeForegroundService));
    //        context.StopService(intent);
    //    }
    //}
    public static class ServiceHelper
    {
        public static T GetService<T>() =>
            MauiApplication.Current.Services.GetRequiredService<T>();

        public static IServiceProvider Current =>
            IPlatformApplication.Current?.Services
            ?? throw new InvalidOperationException("No service provider.");
    }
}
