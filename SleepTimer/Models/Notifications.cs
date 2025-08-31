using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace SleepTimer.Models
{
    public static class Notifications
    {
        const int notificationId = 100;

        public async static Task Show(NotificationMessage message)
        {

            var notification = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = "Sleep timer",
                Description = message.ToString(),
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
        public static async Task<bool> EnsureNotificationPermissionAsync()
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

    public class NotificationMessage
    {
        protected string Message { get; set; } = string.Empty;
        protected NotificationMessage(string message)
        {
            this.Message = message;
        }
        public override string ToString()
        {
            return Message;
        }
    }
    public class NotificationMessageRemainingTime(int remainingMinutes)
        : NotificationMessage($"{remainingMinutes} minutes left. Tap to extend!") {}
    public class NotificationMessageGoingToSleep()
        : NotificationMessage("Going to sleep.") { }
    public class NotificationMessageCustom(string message)
        : NotificationMessage(message) { }



}
