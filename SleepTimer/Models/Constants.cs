using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public static class Constants
    {
        public const int VolumeStep = 1;
        public const int FinalPhaseSeconds = 60;
    }
}
