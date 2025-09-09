using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public class SleepTimerNotifier
    {
        private readonly AppPreferences appPreferences;
        private int lastNotificationMinute = int.MaxValue;
        private readonly Action<string, NotificationLevel> notify;

        public SleepTimerNotifier(AppPreferences appPreferences, Action<string, NotificationLevel> notify)
        {
            this.appPreferences = appPreferences;
            this.notify = notify;
        }

        public void OnTick(TimeSpan remainingTime)
        {
            if (remainingTime.TotalSeconds <= 0)
                notify("Sleep timer finished.", NotificationLevel.Low);
            else if (remainingTime.Minutes == 0 && remainingTime.Seconds < 10)
                notify("Going to sleep.", NotificationLevel.Low);
            else if (remainingTime.Seconds > 0 && Math.Abs(remainingTime.Minutes - lastNotificationMinute) > 0)
            {
                var highPriorityMinutes = new int[] { 1, 2, 5, 10 };
                if (highPriorityMinutes.Contains(remainingTime.Minutes))
                {
                    notify($"{remainingTime.Minutes} minutes left.", NotificationLevel.High);
                    lastNotificationMinute = remainingTime.Minutes;
                }
            }
        }
    }
}
