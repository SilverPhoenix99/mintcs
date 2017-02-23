using System.Collections.Generic;
using System.Linq;

namespace Mint.MethodBinding.Compilation
{
    internal class CallCompilerCache<T>
    {
        private Dictionary<long, CachedMethod<T>> Cache { get; } = new Dictionary<long, CachedMethod<T>>();

        public int Count => Cache.Count;

        public IEnumerable<CachedMethod<T>> Values => Cache.Values;

        public CachedMethod<T> this[long classId]
        {
            get
            {
                CachedMethod<T> method;
                var foundAndIsValid = Cache.TryGetValue(classId, out method) && method.Binder.Condition.Valid;
                return foundAndIsValid ? method : null;
            }
        }

        public void Put(CachedMethod<T> cachedMethod) => Cache[cachedMethod.ClassId] = cachedMethod;

        public void RemoveInvalidCachedMethods()
        {
            var invalidKeys = Cache.Where(_ => !_.Value.Binder.Condition.Valid).Select(_ => _.Key).ToArray();
            foreach(var key in invalidKeys)
            {
                Cache.Remove(key);
            }
        }
    }
}