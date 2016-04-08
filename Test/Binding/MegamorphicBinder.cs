using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mint;
using static System.Linq.Expressions.Expression;

namespace Mint.Binding
{
    class MegamorphicBinder : Binder
    {
        private Dictionary<long, CachedMethod> cache = new Dictionary<long, CachedMethod>();

        public MegamorphicBinder(Symbol methodName)
        {
            MethodName = methodName;
        }

        public Symbol MethodName { get; }

        public CallSite.Function Compile(CallSite site) => Invoke;

        private iObject Invoke(iObject instance, iObject[] args)
        {
            var klass = instance.CalculatedClass;
            CachedMethod method;
            if(!cache.TryGetValue(klass.Id, out method) || !method.Method.Condition.Valid)
            {
                var uncompiledMethod = klass.FindMethod(MethodName) ?? klass.FindMethod(METHOD_MISSING);
                cache.Remove(klass.Id);
                cache[klass.Id] = method = new CachedMethod(uncompiledMethod);
            }

            return method.CompiledMethod(instance, args);
        }

        class CachedMethod
        {
            public CachedMethod(Method method)
            {
                CompiledMethod = CompileMethod(Method = method);
            }

            public Method Method { get; }
            public CallSite.Function CompiledMethod { get; }

            private static CallSite.Function CompileMethod(Method method)
            {
                var instance = Parameter(typeof(iObject), "instance");
                var args = Parameter(typeof(iObject[]), "args");

                var body = method.Bind(instance, new[] { args });

                var lambda = Lambda<CallSite.Function>(body, instance, args);
                return lambda.Compile();
            }
        }

        #region Static

        public static readonly Symbol METHOD_MISSING;

        static MegamorphicBinder()
        {
            METHOD_MISSING = new Symbol("method_missing");
        }

        #endregion
    }
}