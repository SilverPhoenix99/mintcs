using System;
using System.Collections.Generic;
using System.Linq;

namespace Mint.MethodBinding
{
    public sealed partial class MegamorphicCallCompiler : CallSiteBinder
    {
        private readonly Dictionary<long, CachedMethod> cache;
        
        public CallSite CallSite { get; }

        public MegamorphicCallCompiler(CallSite callSite)
        {
            CallSite = callSite;
            cache = new Dictionary<long, CachedMethod>();
        }

        internal MegamorphicCallCompiler(CallSite callSite, IEnumerable<KeyValuePair<long, MethodBinder>> cache)
            : this(callSite)
        {
            this.cache = cache.ToDictionary(_ => _.Key, _ => new CachedMethod(_.Value, callSite));
        }

        public Function Compile() => Call;

        private iObject Call(iObject instance, iObject[] args)
        {
            var klass = instance.CalculatedClass;
            CachedMethod method;
            if(!cache.TryGetValue(klass.Id, out method) || !method.Binder.Condition.Valid)
            {
                Cleanup();
                var binder = Object.FindMethod(instance, CallSite.MethodName, args);
                if(binder == null)
                {
                    throw new InvalidOperationException($"No method found for {instance.CalculatedClass.FullName}");
                }
                cache[klass.Id] = method = new CachedMethod(binder, CallSite);
            }

            return method.Call(instance, args);
        }

        private void Cleanup()
        {
            var invalidKeys = cache.Where(_ => !_.Value.Binder.Condition.Valid)
                                   .Select(_ => _.Key)
                                   .ToArray();

            foreach(var key in invalidKeys)
            {
                cache.Remove(key);
            }
        }
    }
}