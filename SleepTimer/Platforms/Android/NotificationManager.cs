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
        private readonly int serviceId;

        public NotificationManager(Context context, int serviceId)
        {
            this.context = context;
            this.serviceId = serviceId;
        }

        public void Show(string message, NotificationLevel level = NotificationLevel.High)
        {
            var notification = BuildNotification(message, level);
            (context as Service)?.StartForeground(serviceId, notification);
        }

        public void Update(string message, NotificationLevel level = NotificationLevel.High)
        {
            var notification = BuildNotification(message, level);
            NotificationManagerCompat.From(context).Notify(serviceId, notification);
        }

        public void Clear()
        {
            NotificationManagerCompat.From(context).Cancel(serviceId);
        }

        private Notification BuildNotification(string message, NotificationLevel level)
        {
            // Your existing BuildNotification code moved here
            // Keeps Android-specific notification logic isolated
            throw new NotImplementedException();
        }
    }
}
