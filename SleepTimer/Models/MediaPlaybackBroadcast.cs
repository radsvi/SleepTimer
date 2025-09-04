using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public interface IMediaPlaybackBroadcast
    {
        void SendMediaPause();
        void SendMediaStop();
    }
    public class StubMediaPlaybackBroadcast : IMediaPlaybackBroadcast
    {
        public void SendMediaPause()
        {
            throw new NotImplementedException();
        }

        public void SendMediaStop()
        {
            throw new NotImplementedException();
        }
    }
}
