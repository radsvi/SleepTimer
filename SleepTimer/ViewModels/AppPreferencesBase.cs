using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.ViewModels
{
    public abstract class AppPreferencesBase : ObservableObject
    {
        protected bool SetProperty<T>(
            ref PreferencesObject<T> store,
            T value,
            [CallerMemberName] string propertyName = null!)
        {
            if (!EqualityComparer<T>.Default.Equals(store.Value, value))
            {
                store.Value = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }
        protected bool SetProperty<T>(
            ref PreferencesObservableCollection<T> store,
            PreferencesObservableCollection<T> value,
            [CallerMemberName] string propertyName = null!)
        {
            if (!ReferenceEquals(store, value))
            {
                store = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }
    }
}
