using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Test
{
    public class LinkedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, LinkedListNode<Tuple<TKey, TValue>>> map;
        private readonly LinkedList<Tuple<TKey, TValue>> list = new LinkedList<Tuple<TKey, TValue>>();

        public LinkedDictionary()
        {
            map = new Dictionary<TKey, LinkedListNode<Tuple<TKey, TValue>>>();
        }

        public LinkedDictionary(int capacity)
        {
            map = new Dictionary<TKey, LinkedListNode<Tuple<TKey, TValue>>>(capacity);
        }

        public LinkedDictionary(IEqualityComparer<TKey> comparer)
        {
            map = new Dictionary<TKey, LinkedListNode<Tuple<TKey, TValue>>>(comparer);
        }

        public LinkedDictionary(LinkedDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer = null)
            : this(comparer)
        {
            foreach(var item in dictionary)
            {
                Add(item);
            }
        }
        
        public int Count                  => map.Count;
        public bool IsReadOnly            => false;
        public ICollection<TKey> Keys     => map.Keys;
        public ICollection<TValue> Values => new ReadOnlyCollection<TValue>(map.Values.Select(_ => _.Value.Item2).ToList());

        public TValue this[TKey key]
        {
            get { return map[key].Value.Item2; }
            set { Add(key, value); }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
            list.Select(_ => new KeyValuePair<TKey, TValue>(_.Item1, _.Item2)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            map.Clear();
            list.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key) => map.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value)
        {
            LinkedListNode<Tuple<TKey, TValue>> val;
            if(map.TryGetValue(key, out val))
            {
                value = val.Value.Item2;
                return true;
            }

            value = default(TValue);
            return false;
        }
    }
}
