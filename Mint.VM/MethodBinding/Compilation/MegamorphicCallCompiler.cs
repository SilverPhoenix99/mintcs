using System;
using Mint.MethodBinding.Arguments;
using Mint.MethodBinding.Methods;
using System.Collections.Generic;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Compilation
{
    public sealed class MegamorphicCallCompiler : BaseCallCompiler
    {
        public MegamorphicCallCompiler(CallSite callSite)
            : base(callSite)
        {
            Cache = new CallCompilerCache<CallSite.Stub>();
        }


        internal MegamorphicCallCompiler(CallSite callSite, IEnumerable<CachedMethod> cache)
            : this(callSite)
        {
            foreach(var cachedMethod in cache)
            {
                if(cachedMethod.Binder.Condition.Valid)
                {
                    var classId = cachedMethod.ClassId;
                    var instanceType = cachedMethod.InstanceType;
                    var binder = cachedMethod.Binder;
                    Cache.Put(CreateCachedMethod(classId, instanceType, binder));
                }
            }
        }


        private CallCompilerCache<CallSite.Stub> Cache { get; }


        public override CallSite.Stub Compile()
            => Call;


        private iObject Call(iObject instance, ArgumentBundle bundle)
        {
            var classId = instance.EffectiveClass.Id;
            var cachedMethod = Cache[classId];

            if(cachedMethod == null || !cachedMethod.Binder.Condition.Valid)
            {
                Cache.RemoveInvalidCachedMethods();
                var binder = TryFindMethodBinder(instance);
                cachedMethod = CreateCachedMethod(classId, instance.GetType(), binder);
                Cache.Put(cachedMethod);
            }

            return cachedMethod.CachedCall(instance, bundle);
        }

 
        private CachedMethod<CallSite.Stub> CreateCachedMethod(long classId, Type instanceType, MethodBinder binder)
        {
            var stub = CompileMethod(instanceType, binder);
            return new CachedMethod<CallSite.Stub>(classId, instanceType, binder, stub);
        }


        private CallSite.Stub CompileMethod(Type instanceType, MethodBinder binder)
        {
            var instanceVariable = Parameter(typeof(iObject), "instance");
            var bundle = Parameter(typeof(ArgumentBundle), "bundle");
            var frame = new CallFrameBinder(CallSite, instanceType, instanceVariable, bundle);
            var body = binder.Bind(frame);
            var lambda = Lambda<CallSite.Stub>(body, instanceVariable, bundle);
            return lambda.Compile();
        }
   }
}