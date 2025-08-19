using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace SleepTimer.Models
{
    public static class Notifications
    {
        
        public async static Task Show(int minutesLeft)
        {
            if (minutesLeft < 0)
                return;

            var notification = new NotificationRequest
            {
                NotificationId = 100,
                Title = "Sleep timer!",
                Description = "X minutes left. Tap to extend!",
                Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions
                {
                    Ongoing = true,
                    AutoCancel = false,
                    ChannelId = "default",
                    Priority = AndroidPriority.High
                }
            };

            await EnsureNotificationPermissionAsync();

            await LocalNotificationCenter.Current.Show(notification);
        }
        async static Task<bool> EnsureNotificationPermissionAsync()
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
