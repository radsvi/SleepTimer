using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public interface IAudioFocusService
    {
        void RequestAudioFocus();
        void AbandonAudioFocus();
    }
    public class StubAudioFocusService : IAudioFocusService
    {
        public void AbandonAudioFocus()
        {
            throw new NotImplementedException();
        }

        public void RequestAudioFocus()
        {
            throw new NotImplementedException();
        }
    }
}
