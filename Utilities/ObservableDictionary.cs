using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.App.Utilities
{
    public class ObservableDictionary<TKey, TValue> :
    ObservableCollection<KeyValuePair<TKey, TValue>>,
    INotifyPropertyChanged
    {
        private readonly Dictionary<TKey, int> _keyIndexMap = new();

        public ObservableDictionary() =>
            CollectionChanged += OnCollectionChanged;

        public void AddOrUpdate(TKey key, TValue value)
        {
            if (_keyIndexMap.TryGetValue(key, out int index))
            {
                // Update existing
                this[index] = new KeyValuePair<TKey, TValue>(key, value);
            }
            else
            {
                // Add new
                Add(new KeyValuePair<TKey, TValue>(key, value));
            }
        }

        public bool Remove(TKey key)
        {
            if (!_keyIndexMap.TryGetValue(key, out int index))
                return false;

            RemoveAt(index);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_keyIndexMap.TryGetValue(key, out int index))
            {
                value = this[index].Value;
                return true;
            }
            value = default;
            return false;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems.Cast<KeyValuePair<TKey, TValue>>())
                    {
                        _keyIndexMap[item.Key] = Count - 1;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RebuildIndex();
                    break;

                case NotifyCollectionChangedAction.Reset:
                    _keyIndexMap.Clear();
                    break;
            }
        }

        private void RebuildIndex()
        {
            _keyIndexMap.Clear();
            for (int i = 0; i < Count; i++)
            {
                _keyIndexMap[this[i].Key] = i;
            }
        }
    }
}
