using System;
using System.Collections.Generic;
using System.Linq;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.CallCompilation
{
    public sealed class MegamorphicCallCompiler : CallCompiler
    {
        private readonly Dictionary<long, CachedMethod<Function>> cache;
        
        public CallSite CallSite { get; }

        public MegamorphicCallCompiler(CallSite callSite)
        {
            CallSite = callSite;
            cache = new Dictionary<long, CachedMethod<Function>>();
        }

        internal MegamorphicCallCompiler(CallSite callSite, IEnumerable<KeyValuePair<long, MethodBinder>> cache)
            : this(callSite)
        {
            foreach(var pair in cache)
            {
                this.cache.Add(pair.Key, CreateCachedMethod(pair.Key, pair.Value));
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
            var body = binder.Bind(CallSite, instance, arguments);
            var lambda = Lambda<Function>(body, instance, arguments);
            return lambda.Compile();
        }

        public Function Compile() => Call;

        private iObject Call(iObject instance, iObject[] arguments)
        {
            var classId = instance.CalculatedClass.Id;
            CachedMethod<Function> method;
            var foundAndIsValid = cache.TryGetValue(classId, out method) && method.Binder.Condition.Valid;

            if(!foundAndIsValid)
            {
                cache[classId] = method = FindMethodInInstance(instance);
            }

            return method.CachedCall(instance, arguments);
        }

        private CachedMethod<Function> FindMethodInInstance(iObject instance)
        {
            RemoveInvalidCachedMethods();
            var binder = instance.CalculatedClass.FindMethod(CallSite.CallInfo.MethodName);
            if(binder == null)
            {
                throw new InvalidOperationException($"No method found for {instance.CalculatedClass.FullName}");
            }
            return CreateCachedMethod(instance.CalculatedClass.Id, binder);
        }

        private void RemoveInvalidCachedMethods()
        {
            var invalidKeys = cache.Where(_ => !_.Value.Binder.Condition.Valid).Select(_ => _.Key).ToArray();
            foreach(var key in invalidKeys)
            {
                cache.Remove(key);
            }
        }
    }
}