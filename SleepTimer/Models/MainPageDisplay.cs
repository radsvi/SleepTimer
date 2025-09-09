using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public partial class MainPageDisplay : ObservableObject
    {
        private TimeSpan remainingTime = TimeSpan.Zero;
        public TimeSpan RemainingTime
        {
            get => remainingTime;
            set { remainingTime = value; OnPropertyChanged(); }
        }
        public void SetStartTime(int minutes)
        {
            RemainingTime = new TimeSpan(0, minutes, 0);
        }
        public void OnTick(TimeSpan remainingTime)
        {
            RemainingTime = remainingTime;
        }
    }
}
