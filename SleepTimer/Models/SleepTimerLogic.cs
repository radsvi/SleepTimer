using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public interface ISleepTimerLogic
    {
        Task OnTimerElapsedAsync();
        Task OnPostponeAsync();
    }
    public class SleepTimerLogic : ISleepTimerLogic
    {
        public Task OnTimerElapsedAsync()
        {
            // Example: lower volume, stop playback, etc.
            //Console.WriteLine("Timer elapsed -> Lower volume.");
            return Task.CompletedTask;
        }

        public Task OnPostponeAsync()
        {
            // Example: reset media playback inactivity
            //Console.WriteLine("Timer postponed -> Restart countdown.");
            return Task.CompletedTask;
        }
    }
}
