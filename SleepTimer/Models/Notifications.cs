using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace SleepTimer.Models
{
    public enum NotificationMsg {
        RemainingTime,
        GoingToSleep
    }
    public static class Notifications
    {
        const int notificationId = 100;


        public async static Task Show(NotificationMsg msg)
        {
            if (msg == NotificationMsg.GoingToSleep)
                await Show(msg, 0);
        }
        public async static Task Show(NotificationMsg msg, int remainingMinutes)
        {
            string message;
            if (msg == NotificationMsg.RemainingTime)
                message = $"{remainingMinutes} minutes left. Tap to extend!";
            else
                message = "Going to sleep.";

            var notification = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = "Sleep timer!",
                Description = message,
                Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions
                {
                    Ongoing = true,
                    AutoCancel = false,
                    ChannelId = "silent_channel",
                    Priority = AndroidPriority.Min,
                    
                }
            };

            await EnsureNotificationPermissionAsync();

            await LocalNotificationCenter.Current.Show(notification);
        }
        public static void Cancel()
        {
            LocalNotificationCenter.Current.Cancel(notificationId);
        }
        static async Task<bool> EnsureNotificationPermissionAsync()
        {
            var enabled = await LocalNotificationCenter.Current.AreNotificationsEnabled();
            if (enabled)
                return true;

            var granted = await LocalNotificationCenter.Current.RequestNotificationPermission(
                new NotificationPermission
                {
                    Android = { RequestPermissionToScheduleExactAlarm = false }
                });

            return granted;
        }
    }
}
