using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using AndroidX.Core.App;

namespace SleepTimer.Platforms.Android
{
    public class NotificationManager : INotificationManager
    {
        private readonly Context context;
        const int SERVICE_ID = 1001;

        public NotificationManager(Context context)
        {
            this.context = context;
        }

        public void Show(string message, NotificationLevel level = NotificationLevel.High)
        {
            var notification = BuildNotification(message, level);
            (context as Service)?.StartForeground(SERVICE_ID, notification);
        }

        public void Update(string message, NotificationLevel level = NotificationLevel.High)
        {
            var notification = BuildNotification(message, level);
            NotificationManagerCompat.From(context).Notify(SERVICE_ID, notification);
        }
        private void UpdateNotification(string remainingTime, NotificationLevel notificationLevel = NotificationLevel.High)
        {
            var notification = BuildNotification(remainingTime, notificationLevel);
            var manager = NotificationManagerCompat.From(this);
            manager.Notify(SERVICE_ID, notification);
        }

        public void Clear()
        {
            NotificationManagerCompat.From(context).Cancel(SERVICE_ID);
        }

        private Notification BuildNotification(string message, NotificationLevel notificationLevel = NotificationLevel.High)
        {
#pragma warning disable CA1416, CA1422
            var channelId = "sleep_timer_channel";

            var builder = Build.VERSION.SdkInt >= BuildVersionCodes.O
                ? new Notification.Builder(this, channelId)
                : new Notification.Builder(this);

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
                //System.Diagnostics.Debug.WriteLine("notification: " + MapToImportance(notificationLevel));
                var manager = (global::Android.App.NotificationManager?)base.GetSystemService(NotificationService)
                    ?? throw new InvalidOperationException("NotificationManager not available");
                manager.CreateNotificationChannel(channel);
            }

            var extendIntent = new Intent(this, typeof(SleepTimerService));
            extendIntent.SetAction(ServiceAction.Extend.ToString());
            var extendPending = PendingIntent.GetService(this, 1, extendIntent, PendingIntentFlags.Immutable);

            var stopIntent = new Intent(this, typeof(SleepTimerService));
            stopIntent.SetAction(ServiceAction.Stop.ToString());
            var stopPending = PendingIntent.GetService(this, 2, stopIntent, PendingIntentFlags.Immutable);

            builder
                .SetContentIntent(extendPending)
                .AddAction(new Notification.Action.Builder(0, ServiceAction.Stop.ToString(), stopPending).Build());

            var notification = builder.Build();

            return notification;
#pragma warning restore CA1416, CA1422
        }
    }
}
