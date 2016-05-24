using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mint.Binding.Compilation
{
    internal class CallCompilerCache<T> : IEnumerable<KeyValuePair<long, CachedMethod<T>>>
    {
        private readonly Dictionary<long, CachedMethod<T>> cache = new Dictionary<long, CachedMethod<T>>();

        public int Count => cache.Count;

        public CachedMethod<T> this[long classId]
        {
            get
            {
                CachedMethod<T> method;
                var foundAndIsValid = cache.TryGetValue(classId, out method) && method.Binder.Condition.Valid;
                return foundAndIsValid ? method : null;
            }
        }

        public void Put(CachedMethod<T> cachedMethod) => cache[cachedMethod.ClassId] = cachedMethod;

        public void RemoveInvalidCachedMethods()
        {
            var invalidKeys = cache.Where(_ => !_.Value.Binder.Condition.Valid).Select(_ => _.Key).ToArray();
            foreach(var key in invalidKeys)
            {
                cache.Remove(key);
            }
        }

        public IEnumerator<KeyValuePair<long, CachedMethod<T>>> GetEnumerator() => cache.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}