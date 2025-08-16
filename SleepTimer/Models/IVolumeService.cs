using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public interface IVolumeService
    {
        void SetVolume(int level);
        int GetVolume();
    }
    public class StubVolumeService : IVolumeService
    {
        public void SetVolume(int level) { }
        public int GetVolume() => -1;
    }
}
