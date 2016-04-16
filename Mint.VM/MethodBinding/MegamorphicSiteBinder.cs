using System;
using System.Collections.Generic;
using System.Linq;

namespace Mint.MethodBinding
{
    public sealed partial class MegamorphicSiteBinder : CallSiteBinder
    {
        private readonly Dictionary<long, CachedMethod> cache;

        public MegamorphicSiteBinder()
        {
            cache = new Dictionary<long, CachedMethod>();
        }

        internal MegamorphicSiteBinder(Dictionary<long, PolymorphicSiteBinder.CachedMethod> cache, CallSite site)
        {
            this.cache = cache.ToDictionary(_ => _.Key, _ => new CachedMethod(_.Value.Binder, site));
        }

        public Function Compile(CallSite site)
        {
            return (instance, args) => Invoke(site, instance, args);
        }

        private iObject Invoke(CallSite site, iObject instance, iObject[] args)
        {
            var klass = instance.CalculatedClass;
            CachedMethod method;
            if(!cache.TryGetValue(klass.Id, out method) || !method.Binder.Condition.Valid)
            {
                Cleanup();
                var binder = Object.FindMethod(instance, site.MethodName, args);
                if(binder == null)
                {
                    throw new InvalidOperationException($"No method found for {instance.CalculatedClass.FullName}");
                }
                cache[klass.Id] = method = new CachedMethod(binder, site);
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