using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    //public class PreferencesObject<T>
    //{
    //    private readonly string propertyName;

    //    public PreferencesObject([CallerMemberName] string propertyName = null)
    //    {
    //        this.propertyName = propertyName;

    //        string serialized = Preferences.Default.Get(this.propertyName, string.Empty);
    //        //if (!string.IsNullOrWhiteSpace(serialized))
    //        if (serialized != string.Empty)
    //        {
    //            var items = JsonConvert.DeserializeObject<List<T>>(serialized);
    //            if (items != null)
    //            {
    //                foreach (var item in items)
    //                    Add(item);
    //            }
    //        }

    //        CollectionChanged += (s, e) => Save();
    //    }

    //    private void Save()
    //    {
    //        string serialized = JsonConvert.SerializeObject(this.ToList());
    //        Preferences.Default.Set(propertyName, serialized);
    //    }
    //}


    public class PersistedProperty<T>
    {
        private readonly string _key;
        private T _value;

        public PersistedProperty(
            T defaultValue = default!,
            [CallerMemberName] string propertyName = "",
            string? key = null)
        {
            _key = key ?? propertyName;

            // Load from preferences
            if (Preferences.Default.ContainsKey(_key))
            {
                if (typeof(T).IsPrimitive || typeof(T) == typeof(string) || typeof(T) == typeof(decimal))
                {
                    // For primitive types, store directly
                    object? stored = Preferences.Default.Get(_key, defaultValue?.ToString() ?? string.Empty);
                    _value = (T)Convert.ChangeType(stored, typeof(T));
                }
                else
                {
                    // For complex types, use JSON
                    string serialized = Preferences.Default.Get(_key, string.Empty);
                    if (!string.IsNullOrWhiteSpace(serialized))
                    {
                        _value = JsonConvert.DeserializeObject<T>(serialized) ?? defaultValue;
                    }
                    else
                    {
                        _value = defaultValue;
                    }
                }
            }
            else
            {
                _value = defaultValue;
            }
        }

        public T Value
        {
            get => _value;
            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                    Save();
                }
            }
        }

        private void Save()
        {
            if (typeof(T).IsPrimitive || typeof(T) == typeof(string) || typeof(T) == typeof(decimal))
            {
                Preferences.Default.Set(_key, _value?.ToString() ?? string.Empty);
            }
            else
            {
                string serialized = JsonConvert.SerializeObject(_value);
                Preferences.Default.Set(_key, serialized);
            }
        }

        // Implicit cast so you can use it as T
        public static implicit operator T(PersistedProperty<T> p) => p.Value;
    }
}
