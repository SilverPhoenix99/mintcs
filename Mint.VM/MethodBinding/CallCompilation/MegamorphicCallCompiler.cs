using System;
using System.Collections.Generic;
using Mint.MethodBinding.Binders;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.CallCompilation
{
    public sealed class MegamorphicCallCompiler : CallCompiler
    {
        private readonly CallCompilerCache<Function> cache;
        
        public CallSite CallSite { get; }

        public MegamorphicCallCompiler(CallSite callSite)
        {
            CallSite = callSite;
            cache = new CallCompilerCache<Function>();
        }

        internal MegamorphicCallCompiler(CallSite callSite, IEnumerable<KeyValuePair<long, MethodBinder>> cache)
            : this(callSite)
        {
            foreach(var pair in cache)
            {
                this.cache.Put(CreateCachedMethod(pair.Key, pair.Value));
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
            var body = binder.Bind(CallSite.CallInfo, instance, arguments);
            var lambda = Lambda<Function>(body, instance, arguments);
            return lambda.Compile();
        }

        public Function Compile() => Call;

        private iObject Call(iObject instance, iObject[] arguments)
        {
            var classId = instance.EffectiveClass.Id;
            var method = cache[classId];

            if(method == null)
            {
                cache.Put(method = FindMethodInInstance(instance));
            }

            return method.CachedCall(instance, arguments);
        }

        private CachedMethod<Function> FindMethodInInstance(iObject instance)
        {
            cache.RemoveInvalidCachedMethods();
            var binder = instance.EffectiveClass.FindMethod(CallSite.CallInfo.MethodName);
            if(binder == null)
            {
                throw new InvalidOperationException($"No method found for {instance.EffectiveClass.FullName}");
            }
            return CreateCachedMethod(instance.EffectiveClass.Id, binder);
        }
    }
}