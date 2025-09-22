using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public class PreferencesLinkedList<T> : ObservableObject, IEnumerable<T>, INotifyCollectionChanged
    {
        private readonly string key;
        private LinkedList<T> value;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public int Count => value.Count;

        public PreferencesLinkedList([CallerMemberName] string propertyName = null)
        {
            this.key = propertyName;
            value = Load();
        }

        // Commenting out OnPropertyChanged for Adding values as I only care about these for the cleanup.
        public void AddFirst(T item)
        {
            value.AddFirst(item);
            Save();
            //OnPropertyChanged(nameof(Count));
            //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }
        public void AddLast(T item)
        {
            value.AddLast(item);
            Save();
            //OnPropertyChanged(nameof(Count));
            //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }
        public void Clear()
        {
            value.Clear();
            Save();
            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) =>
            CollectionChanged?.Invoke(this, e);
    }
}
