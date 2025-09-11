using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public interface INotificationManager
    {
        void Show(string message, NotificationLevel level = NotificationLevel.High);
        void Update(string message, NotificationLevel level = NotificationLevel.High);
        void Clear();
    }
}
