using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public class PreferencesObject<T>
    {
        private readonly string key;
        private T value;
        public T Value
        {
            get => value;
            set
            {
                if (!Equals(this.value, value))
                {
                    this.value = value;
                    Save();
                }
            }
        }
        public static implicit operator T(PreferencesObject<T> p) => p.Value;

        public PreferencesObject(
            T defaultValue = default!,
            [CallerMemberName] string propertyName = "",
            string? key = null)
        {
            this.key = key ?? propertyName;

            if (Preferences.Default.ContainsKey(this.key))
            {
                if (PreferencesObject<T>.IsNativeType())
                {
                    value = Preferences.Default.Get(this.key, defaultValue);
                }
                else
                {
                    string serialized = Preferences.Default.Get(this.key, string.Empty);
                    value = string.IsNullOrWhiteSpace(serialized)
                        ? defaultValue
                        : JsonConvert.DeserializeObject<T>(serialized) ?? defaultValue;
                }
            }
            else
            {
                value = defaultValue;
            }
        }



        private void Save()
        {
            if (IsNativeType())
            {
                Preferences.Default.Set(key, value?.ToString() ?? string.Empty);
            }
            else
            {
                string serialized = JsonConvert.SerializeObject(value);
                Preferences.Default.Set(key, serialized);
            }
        }


        private static bool IsNativeType()
        {
            return typeof(T).IsPrimitive || typeof(T) == typeof(string) || typeof(T) == typeof(decimal);
        }
    }
}
