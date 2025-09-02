using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public interface IGradualVolumeService
    {
        int GetVolume();
        void SetVolume(int volume);
        //void LowerVolume();

    }
    internal class StubGradualVolumeService : IGradualVolumeService
    {
        public int GetVolume()
        {
            throw new NotImplementedException();
        }

        public void SetVolume(int volume)
        {
            throw new NotImplementedException();
        }
    }
}
