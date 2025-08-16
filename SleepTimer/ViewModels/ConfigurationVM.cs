using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.ViewModels
{
    public partial class ConfigurationVM(AppPreferences appPreferences) : ObservableObject
    {
        public AppPreferences AppPreferences { get; } = appPreferences;
    }
}
