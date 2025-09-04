using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Services
{
    public enum ServiceAction
    {
        Start,
        Extend,
        Stop
    }
    public interface ISleepTimerServiceHelper
    {
        void SleepTimerControl(ServiceAction action);
    }
    public class StubSleepTimerServiceHelper : ISleepTimerServiceHelper
    {
        public void SleepTimerControl(ServiceAction action)
        {
            throw new NotImplementedException();
        }
    }
}
