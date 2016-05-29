using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mint
{
    public class Hash : BaseObject, IEnumerable<iObject>
    {
        private readonly LinkedDictionary<iObject, iObject> map;

        public IEnumerable<iObject> Keys => map.Keys;

        public IEnumerable<iObject> Values => map.Values;

        public int Count => map.Count;

        public Hash() : base(Class.HASH)
        {
            map = new LinkedDictionary<iObject, iObject>();
        }

        public Hash(int capacity) : base(Class.HASH)
        {
            map = new LinkedDictionary<iObject, iObject>(capacity);
        }

        internal Hash(IDictionary<iObject, iObject> map) : this(map.Count)
        {
            foreach(var pair in map)
            {
                this[pair.Key] = pair.Value;
            }
        }

        public iObject this[iObject key]
        {
            get
            {
                iObject value;
                map.TryGetValue(key, out value);
                return value;
            }
            set { map[key] = value; }
        }

        public Array ToArray() => new Array(map.Select(_ => new Array(_.Key, _.Value)));

        public IEnumerator<iObject> GetEnumerator()
        {
            foreach(var element in map.Select(_ => (iObject) new Array(_.Key, _.Value)))
            {
                yield return element;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString()
        {
            var elements = map.Select(_ => $"{_.Key.Inspect()}=>{_.Value.Inspect()}");
            return $"{{{string.Join(", ", elements)}}}";
        }

        public Hash MergeSelf(Hash otherHash)
        {
            foreach(var element in otherHash.map)
            {
                map.Add(element);
            }

            return this;
        }

        public Hash Merge(Hash otherHash) => new Hash(map).MergeSelf(otherHash);
    }
}