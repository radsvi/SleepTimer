using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.ViewModels
{
    public partial class MainVM : ObservableObject
    {
        public AppPreferences AppPreferences { get; }
        public MainVM(AppPreferences appPreferences)
        {
            this.AppPreferences = appPreferences;
        }

        public double MyDuration { get; set; } = 21.5;
    }
}
