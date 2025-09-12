using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public class TimerNotifier
    {
        private readonly AppPreferences appPreferences;
        private TimeSpan nextNotificationTime = TimeSpan.MinValue;
        private readonly Action<string, NotificationLevel> notify;

        public TimerNotifier(AppPreferences appPreferences, Action<string, NotificationLevel> notify)
        {
            this.appPreferences = appPreferences;
            this.notify = notify;
        }
        public void OnStart(TimeSpan startingTime) // this is needed when we start the timer and its already in fade-out time.
        {
            nextNotificationTime = startingTime;
        }
        public void OnTick(TimeSpan remainingTime)
        {
            if (remainingTime.TotalSeconds <= 0)
                notify("Sleep timer in stand by.", NotificationLevel.Low);
            else if (remainingTime.TotalSeconds < 10)
                notify("Going to sleep.", NotificationLevel.Low);
            else if (remainingTime.TotalSeconds < appPreferences.FadeOutSeconds)
                notify($"{(int)remainingTime.TotalSeconds} seconds left.", NotificationLevel.Low); 
            else if (remainingTime.Seconds > 55 && remainingTime < NextNotification(nextNotificationTime)
                || remainingTime > nextNotificationTime)
            {
                int roundUpMinutes = (int)Math.Ceiling(remainingTime.TotalMinutes);
                NotificationLevel chosenPriority;
                if (roundUpMinutes == 1 || (roundUpMinutes % 5 == 0))
                    chosenPriority = NotificationLevel.High;
                else
                    chosenPriority = NotificationLevel.Low;

                notify($"{roundUpMinutes} minutes left.", chosenPriority);
                //nextNotificationTime = NextNotification(remainingTime);
                nextNotificationTime = remainingTime;
            }
        }
        private static TimeSpan NextNotification(TimeSpan time) => time.Add(new TimeSpan(0, 0, -55));
    }
}
