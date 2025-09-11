using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using AndroidX.Core.App;

using SleepTimer.Platforms.Android.Services;

namespace SleepTimer.Platforms.Android
{
    public class NotificationManagerWrapper //: INotificationManagerWrapper
    {
        //private readonly SleepTimerService context;
        private readonly Context context;

        public NotificationManagerWrapper(Context context)
        {
            this.context = context;
        }

        public void Show(string message, NotificationLevel level = NotificationLevel.High)
        {
            var notification = BuildNotification(message, level);
            (context as Service)?.StartForeground(Constants.SERVICE_ID, notification);
        }

        public void Update(string message, NotificationLevel level = NotificationLevel.High)
        {
            var notification = BuildNotification(message, level);
            var manager = NotificationManagerCompat.From(context) ?? throw new NullReferenceException();
            manager.Notify(Constants.SERVICE_ID, notification);
        }

        public void Clear()
        {
            var manager = NotificationManagerCompat.From(context) ?? throw new NullReferenceException();
            manager.Cancel(Constants.SERVICE_ID);
        }

        private Notification BuildNotification(string message, NotificationLevel notificationLevel = NotificationLevel.High)
        {
#pragma warning disable CA1416, CA1422
            var channelId = "sleep_timer_channel";

            var builder = Build.VERSION.SdkInt >= BuildVersionCodes.O
                ? new Notification.Builder(context, channelId)
                : new Notification.Builder(context);

            builder.SetContentTitle("Sleep Timer")
                   .SetContentText($"{message} Tap to extend!")
                   .SetSmallIcon(global::Android.Resource.Drawable.IcLockIdleAlarm);

            // Handle priority for pre-26
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                builder.SetPriority((int)NotificationLevelHelper.MapToPriority(notificationLevel)); // silent but visible
                //System.Diagnostics.Debug.WriteLine("notification: " + MapToPriority(notificationLevel));
            }
            else
            {
                // Ensure the channel exists for API 26+
                var channel = new NotificationChannel(
                    channelId,
                    "Sleep Timer",
                    NotificationLevelHelper.MapToImportance(notificationLevel) //NotificationImportance.High
                );
                channel.SetSound(null, null);
                NotificationManager manager = (NotificationManager?)context.GetSystemService(Context.NotificationService)
                    ?? throw new InvalidOperationException();
                manager.CreateNotificationChannel(channel);
            }

            var extendIntent = new Intent(context, typeof(SleepTimerService));
            extendIntent.SetAction(ServiceAction.Extend.ToString());
            var extendPending = PendingIntent.GetService(context, 1, extendIntent, PendingIntentFlags.Immutable);

            var stopIntent = new Intent(context, typeof(SleepTimerService));
            stopIntent.SetAction(ServiceAction.Stop.ToString());
            var stopPending = PendingIntent.GetService(context, 2, stopIntent, PendingIntentFlags.Immutable);

            builder
                .SetContentIntent(extendPending)
                .AddAction(new Notification.Action.Builder(0, ServiceAction.Stop.ToString(), stopPending).Build());

            var notification = builder.Build();

            return notification;
#pragma warning restore CA1416, CA1422
        }
    }
}
