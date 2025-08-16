using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public partial class AppPreferences : ObservableObject
    {
        public int DefaultDuration
        {
            get => Preferences.Default.Get(nameof(DefaultDuration), 12);
            set { Preferences.Set(nameof(DefaultDuration), value); OnPropertyChanged(); }
        }
        public int ExtensionLength
        {
            get => Preferences.Default.Get(nameof(ExtensionLength), 5);
            set { Preferences.Set(nameof(ExtensionLength), value); OnPropertyChanged(); }
        }
    }
}
