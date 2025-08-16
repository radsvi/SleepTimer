using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SleepTimer.Models
{
    public class MainTimer
    {
        public System.Timers.Timer TimerToken { get; private set; } = new System.Timers.Timer();
        public MainTimer()
        {
            TimerToken.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            TimerToken.Interval = 5000; // ~ 5 seconds
            
        }
        //public bool IsStarted { get; private set; }

        private void OnTimedEvent(object? source, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }
        public void Start()
        {
            TimerToken.Enabled = true;
        }
        public void Stop()
        {
            TimerToken.Enabled = false;
        }
        public void Extend()
        {
            throw new NotImplementedException();
        }
    }
}
