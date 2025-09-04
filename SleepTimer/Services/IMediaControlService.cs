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
        //bool HasNotificationAccess();
        //void RequestNotificationAccess();
        void CheckNotificationAccess();
    }
    public class StubMediaControlService : IMediaControlService
    {
        public void CheckNotificationAccess()
        {
#warning uncomment throw (commented out only for testing visual styles on windows machine because it compiles faster (+hot reload)
            throw new NotImplementedException();
        }
        public void StopPlayback()
        {
#warning uncomment throw (commented out only for testing visual styles on windows machine because it compiles faster (+hot reload)
            throw new NotImplementedException();
        }
    }
}
