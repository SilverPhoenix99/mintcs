using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mint
{
    public class LinkedDictionary<TKey, TValue> : IDictionary<TKey, TValue>,
                                                  IReadOnlyDictionary<TKey, TValue>,
                                                  IDictionary
    {
        private readonly Dictionary<TKey, Node> map;
        private Node head;

        public LinkedDictionary()
        {
            map = new Dictionary<TKey, Node>();
        }

        public LinkedDictionary(int capacity)
        {
            map = new Dictionary<TKey, Node>(capacity);
        }

        public LinkedDictionary(IEqualityComparer<TKey> comparer)
        {
            map = new Dictionary<TKey, Node>(comparer);
        }

        public LinkedDictionary(LinkedDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer = null)
            : this(comparer)
        {
            foreach(var item in dictionary)
            {
                Add(item);
            }
        }

        public int    Count          => map.Count;
        public bool   IsReadOnly     => false;
        public bool   IsFixedSize    => false;
        public object SyncRoot       => this;
        public bool   IsSynchronized => false;

        public ICollection<TKey> Keys => new KeyCollection(this);

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        ICollection IDictionary.Keys => (ICollection) Keys;

        public ICollection<TValue> Values => new ValueCollection(this);

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        ICollection IDictionary.Values => (ICollection) Values;

        public TValue this[TKey key]
        {
            get { return map[key].Value.Value; }
            set { Add(key, value); }
        }

        public object this[object key]
        {
            get { return ((Node) ((IDictionary) map)[key]).Value.Value; }
            set { Add(key, value); }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for(var node = head; node != null; node = node.Next)
            {
                yield return node.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotSupportedException();
        }

        public void Add(TKey key, TValue value)
        {
            var node = new Node(key, value);
            Remove(key);
            map.Add(key, node);

            if(head == null)
            {
                head = node;
            }
            else
            {
                node.Previous = head.Previous;
                node.Previous.Next = node;
            }

            head.Previous = node;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(object key, object value)
        {
            Add((TKey) key, (TValue) value);
        }

        public void Clear()
        {
            map.Clear();
            head = null;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            return TryGetValue(item.Key, out value)
                && EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        public bool Contains(object item) => item is TKey && ContainsKey((TKey) item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var node = head;
            for(var i = arrayIndex; i < array.Length && node != null; i++)
            {
                array[i] = node.Value;
                node = node.Next;
            }
        }

        public void CopyTo(System.Array array, int arrayIndex)
        {
            var node = head;
            for(var i = arrayIndex; i < array.Length && node != null; i++)
            {
                array.SetValue(node.Value, i);
                node = node.Next;
            }
        }

        public bool Remove(TKey key)
        {
            Node node;
            if(!map.TryGetValue(key, out node))
            {
                return false;
            }
            map.Remove(key);

            if(node == head)
            {
                head = node.Next;
                if(head != null)
                {
                    head.Previous = node.Previous;
                }
            }
            else if(node == head.Previous)
            {
                head.Previous = node.Previous;
                head.Previous.Next = null;
            }
            else
            {
                node.Previous.Next = node.Next;
                node.Next.Previous = node.Previous;
            }

            return true;
        }

        public void Remove(object key)
        {
            Remove((TKey) key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("Please use Remove(TKey) method.");
        }

        public bool ContainsKey(TKey key) => map.ContainsKey(key);

        public bool ContainsValue(TValue value)
        {
            var cmp = EqualityComparer<TValue>.Default;
            return this.Any(_ => cmp.Equals(_.Value, value));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            Node node;
            if(map.TryGetValue(key, out node))
            {
                value = node.Value.Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        private class Node
        {
            public readonly KeyValuePair<TKey, TValue> Value;
            public Node Previous;
            public Node Next;

            public Node(TKey key, TValue value, Node previous = null, Node next = null)
            {
                Value = new KeyValuePair<TKey, TValue>(key, value);
                Previous = previous;
                Next = next;
            }
        }

        private class KeyCollection : ICollection<TKey>
        {
            private readonly LinkedDictionary<TKey, TValue> dictionary;

            public KeyCollection(LinkedDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
            }

            public int  Count      => dictionary.Count;
            public bool IsReadOnly => true;

            public void Add(TKey value)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TKey item) => dictionary.ContainsKey(item);

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                var node = dictionary.head;
                for(var i = arrayIndex; i < array.Length && node != null; i++)
                {
                    array[i] = node.Value.Key;
                    node = node.Next;
                }
            }

            public bool Remove(TKey item)
            {
                throw new NotSupportedException();
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                for(var node = dictionary.head; node != null; node = node.Next)
                {
                    yield return node.Value.Key;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class ValueCollection : ICollection<TValue>
        {
            private readonly LinkedDictionary<TKey, TValue> dictionary;

            public ValueCollection(LinkedDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
            }

            public int  Count      => dictionary.Count;
            public bool IsReadOnly => true;

            public void Add(TValue value)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TValue item) => dictionary.ContainsValue(item);

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                var node = dictionary.head;
                for(var i = arrayIndex; i < array.Length && node != null; i++)
                {
                    array[i] = node.Value.Value;
                    node = node.Next;
                }
            }

            public bool Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                for(var node = dictionary.head; node != null; node = node.Next)
                {
                    yield return node.Value.Value;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
