using System;
using System.Collections.Generic;
using System.Dynamic;
using Mint;

namespace Mint.Binding
{
    class MegamorphicBinder : Binder
    {
        private Dictionary<Class, CachedMethod> cache = new Dictionary<Class, CachedMethod>();

        public MegamorphicBinder(Symbol methodName)
        {
            MethodName = methodName;
        }

        public Symbol MethodName { get; }

        public Func<iObject, iObject[], iObject> Compile(CallSite site) => Invoke;

        private iObject Invoke(iObject instance, iObject[] args)
        {
            var klass = instance.CalculatedClass;
            CachedMethod method;
            if(!cache.TryGetValue(klass, out method) || !method.Condition.Valid)
            {
                var uncompiledMethod = klass.FindMethod(MethodName) ?? klass.FindMethod(METHOD_MISSING);
                cache.Remove(klass);
                cache[klass] = method = new CachedMethod(uncompiledMethod);
            }

            return method.CompiledMethod(instance, args);
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