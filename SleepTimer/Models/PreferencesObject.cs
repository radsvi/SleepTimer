using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public class PreferencesObject<T>
    {
        private readonly string propertyName;
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
            [CallerMemberName] string propertyName = "")
        {
            this.propertyName = propertyName;

            string serialized = Preferences.Default.Get(this.propertyName, string.Empty);
            value = string.IsNullOrWhiteSpace(serialized)
                    ? defaultValue
                    : JsonConvert.DeserializeObject<T>(serialized) ?? defaultValue;
        }
        private void Save() => Preferences.Default.Set(propertyName, JsonConvert.SerializeObject(value));
        //private static bool IsNativeType()
        //{
        //    return typeof(T).IsPrimitive || typeof(T) == typeof(string) || typeof(T) == typeof(decimal);
        //}
    }
}
