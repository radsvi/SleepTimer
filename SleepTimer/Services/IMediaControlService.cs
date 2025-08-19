using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Services
{
    public interface IMediaControlService
    {
        /// <summary>
        /// Requests other apps to pause or stop playback (if supported).
        /// </summary>
        void StopPlayback();
    }
    public class StubMediaControlService : IMediaControlService
    {
        public void StopPlayback()
        {
            // Not supported on this platform
        }
    }
}
