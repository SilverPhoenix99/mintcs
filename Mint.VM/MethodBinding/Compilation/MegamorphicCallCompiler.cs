using Mint.MethodBinding.Arguments;
using Mint.MethodBinding.Methods;
using System.Collections.Generic;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Compilation
{
    public sealed class MegamorphicCallCompiler : BaseCallCompiler
    {
        private readonly CallCompilerCache<CallSite.Stub> cache;

        public MegamorphicCallCompiler(CallSite callSite)
            : base(callSite)
        {
            cache = new CallCompilerCache<CallSite.Stub>();
        }

        internal MegamorphicCallCompiler(CallSite callSite, IEnumerable<KeyValuePair<long, MethodBinder>> cache)
            : this(callSite)
        {
            foreach(var pair in cache)
            {
                if(pair.Value.Condition.Valid)
                {
                    this.cache.Put(CreateCachedMethod(pair.Key, pair.Value));
                }
            }
        }

        private CachedMethod<CallSite.Stub> CreateCachedMethod(long classId, MethodBinder binder)
        {
            var stub = CompileMethod(binder);
            return new CachedMethod<CallSite.Stub>(classId, binder, stub);
        }

        private CallSite.Stub CompileMethod(MethodBinder binder)
        {
            var instance = Parameter(typeof(iObject), "instance");
            var bundle = Parameter(typeof(ArgumentBundle), "bundle");
            var frame = new CallFrameBinder(CallSite, instance, bundle);
            var body = binder.Bind(frame);
            var lambda = Lambda<CallSite.Stub>(body, instance, bundle);
            return lambda.Compile();
        }

        public override CallSite.Stub Compile() => Call;

        private iObject Call(iObject instance, ArgumentBundle bundle)
        {
            var classId = instance.EffectiveClass.Id;
            var cachedMethod = cache[classId];

            if(cachedMethod == null || !cachedMethod.Binder.Condition.Valid)
            {
                cache.RemoveInvalidCachedMethods();
                var binder = TryFindMethodBinder(instance);
                cachedMethod = CreateCachedMethod(instance.EffectiveClass.Id, binder);
                cache.Put(cachedMethod);
            }

            return cachedMethod.CachedCall(instance, bundle);
        }
    }
}