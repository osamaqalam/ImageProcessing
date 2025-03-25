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
    public sealed class MutableKeyValuePair<TKey, TValue> : INotifyPropertyChanged
    {
        public TKey Key { get; }
        private TValue _value;

        public TValue Value
        {
            get => _value;
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public MutableKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            _value = value;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class ObservableDictionary<TKey, TValue> :
    ObservableCollection<MutableKeyValuePair<TKey, TValue>>,
    INotifyPropertyChanged
    {
        private readonly Dictionary<TKey, int> _keyIndexMap = new();

        public ObservableDictionary() =>
            CollectionChanged += OnCollectionChanged;

        public void AddOrUpdate(TKey key, TValue value)
        {
            if (_keyIndexMap.TryGetValue(key, out int index))
            {
                // Update the existing value directly 
                this[index].Value = value;
            }
            else
            {
                // Add a new entry
                Add(new MutableKeyValuePair<TKey, TValue>(key, value));
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
                    int startIndex = e.NewStartingIndex;
                    foreach (var item in e.NewItems.Cast<MutableKeyValuePair<TKey, TValue>>())
                    {
                        _keyIndexMap[item.Key] = startIndex;
                        startIndex++;
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
