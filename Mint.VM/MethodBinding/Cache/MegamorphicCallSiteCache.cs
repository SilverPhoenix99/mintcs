using System;
using Mint.MethodBinding.Methods;
using System.Collections.Generic;

namespace Mint.MethodBinding.Cache
{
    public class MegamorphicCallSiteCache : BaseCallSiteCache
    {
        protected readonly IDictionary<long /* moduleId */, WeakReference<MethodBinder>> cache;
        
        public MegamorphicCallSiteCache(CallSite callSite, IDictionary<long, WeakReference<MethodBinder>> cache = null)
            : base(callSite)
        {
            this.cache = cache != null
                ? new Dictionary<long, WeakReference<MethodBinder>>(cache)
                : new Dictionary<long, WeakReference<MethodBinder>>();
        }

        public override iObject Call()
        {
            var frame = CallFrame.Current;
            var classId = frame.Instance.EffectiveClass.Id;

            if(!(cache.TryGetValue(classId, out var binderRef)
                && binderRef.TryGetTarget(out var binder)
                && binder.Condition.Valid))
            {
                RemoveInvalidCachedMethods();

                binder = TryFindMethodBinder();
                cache[classId] = new WeakReference<MethodBinder>(binder);
            }

            return binder.Call();
        }

        protected void RemoveInvalidCachedMethods()
        {
            var keysToRemove = new List<long>();

            foreach(var methodRef in cache)
            {
                if(!(methodRef.Value.TryGetTarget(out var binder) && binder.Condition.Valid))
                {
                    keysToRemove.Add(methodRef.Key);
                }
            }

            foreach(var key in keysToRemove)
            {
                cache.Remove(key);
            }
        }
    }
}