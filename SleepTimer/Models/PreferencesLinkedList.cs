using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public class PreferencesLinkedList<T> : IEnumerable<T>
    {
        private readonly string key;
        private LinkedList<T> value;

        public PreferencesLinkedList([CallerMemberName] string propertyName = null)
        {
            this.key = propertyName;
            value = Load();
        }

        public void AddFirst(T item)
        {
            value.AddFirst(item);
            Save();
        }
        public void AddLast(T item)
        {
            value.AddLast(item);
            Save();
        }
        public void Clear()
        {
            value.Clear();
            Save();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)value).GetEnumerator();
        }

        private LinkedList<T> Load()
        {
            string serialized = Preferences.Default.Get(key, string.Empty);
            if (serialized == string.Empty)
                return new LinkedList<T>();

            return JsonConvert.DeserializeObject<LinkedList<T>>(serialized)!;
        }
        private void Save()
        {
            string serialized = JsonConvert.SerializeObject(value.ToList());
            Preferences.Default.Set(key, serialized);
        }
    }
}
