using Mint.MethodBinding.Methods;
using System.Collections.Generic;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Compilation
{
    public sealed class MegamorphicCallCompiler : BaseCallCompiler
    {
        private readonly CallCompilerCache<Function> cache;

        public MegamorphicCallCompiler(CallSite callSite)
            : base(callSite)
        {
            cache = new CallCompilerCache<Function>();
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

        private CachedMethod<Function> CreateCachedMethod(long classId, MethodBinder binder)
        {
            var function = CompileMethod(binder);
            return new CachedMethod<Function>(classId, binder, function);
        }

        private Function CompileMethod(MethodBinder binder)
        {
            var instance = Parameter(typeof(iObject), "instance");
            var arguments = Parameter(typeof(iObject[]), "arguments");
            var frame = new CallFrameBinder(CallSite, instance, arguments);
            var body = binder.Bind(frame);
            var lambda = Lambda<Function>(body, instance, arguments);
            return lambda.Compile();
        }

        public override Function Compile() => Call;

        private iObject Call(iObject instance, iObject[] arguments)
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

            return cachedMethod.CachedCall(instance, arguments);
        }
    }
}