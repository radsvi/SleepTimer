using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public struct RemainingTime
    {
        public int TotalMinutes { get; set; }
        public int Seconds { get; set; }
        public RemainingTime(int seconds = 0)
        {
            Seconds = seconds % 60;
            TotalMinutes = seconds / 60;
        }
        public RemainingTime(int totalMinutes, int seconds = 0)
        {
            Seconds = seconds % 60;
            TotalMinutes = seconds / 60 + totalMinutes;
        }
        public RemainingTime(TimeSpan time)
        {
            Seconds = time.Seconds;
            TotalMinutes = (int)time.TotalMinutes;
        }

        public static readonly RemainingTime Zero = new RemainingTime(0);


        public readonly override string ToString()
        {
            return (TotalMinutes >= 100) ? $"{TotalMinutes}" : $"{TotalMinutes}:{Seconds:D2}"; // prevents overlapping on MainPage
        }
    }
}
