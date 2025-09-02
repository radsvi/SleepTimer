using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SleepTimer.Models
{
    public interface ISleepTimerService
    {
        //Task OnTimedEvent(object? sender, System.Timers.ElapsedEventArgs e);
        // Dodelat dalsi
    }
    public class StubSleepTimerService : ISleepTimerService
    {
        //public Task OnTimedEvent(object? sender, ElapsedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
