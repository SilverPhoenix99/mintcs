using System;
using System.Collections.Generic;
using Mint.MethodBinding.Methods;

namespace Mint.MethodBinding.Cache
{
    public class MonomorphicCallSiteCache : BaseCallSiteCache
    {
        private long classId;
        private WeakReference<MethodBinder> binderRef;

        public MonomorphicCallSiteCache(CallSite callSite, long classId, MethodBinder binder)
            : base(callSite)
        {
            this.classId = classId;
            binderRef = new WeakReference<MethodBinder>(binder);
        }
        
        public override iObject Call()
        {
            var currentId = CallFrame.Current.Instance.EffectiveClass.Id;

            if(currentId == classId)
            {
                if(!(binderRef.TryGetTarget(out var binder) && binder.Condition.Valid))
                {
                    binder = TryFindMethodBinder();
                    binderRef = new WeakReference<MethodBinder>(binder);
                }

                return binder.Call();
            }
            
            UpgradeSiteCache(currentId);
            return CallSite.CallCache.Call();
        }

        private void UpgradeSiteCache(long currentId) => CallSite.CallCache = new PolymorphicCallSiteCache(
            CallSite,
            new Dictionary<long, WeakReference<MethodBinder>>
            {
                { classId, binderRef },
                { currentId, new WeakReference<MethodBinder>(TryFindMethodBinder()) }
            }
        );
    }
}
