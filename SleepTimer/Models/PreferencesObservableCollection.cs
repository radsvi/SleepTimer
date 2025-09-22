using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public class PreferencesObservableCollection<T> : ObservableCollection<T>
    {
        private readonly string propertyName;

        public PreferencesObservableCollection([CallerMemberName] string propertyName = null)
        {
            this.propertyName = propertyName;

            string serialized = Preferences.Default.Get(this.propertyName, string.Empty);
            //if (!string.IsNullOrWhiteSpace(serialized))
            if (serialized != string.Empty)
            {
                var items = JsonConvert.DeserializeObject<List<T>>(serialized);
                if (items != null)
                {
                    foreach (var item in items)
                        Add(item);
                }
            }

            CollectionChanged += (s, e) => Save();
        }

        private void Save()
        {
            string serialized = JsonConvert.SerializeObject(this.ToList());
            Preferences.Default.Set(propertyName, serialized);
        }
    }
}