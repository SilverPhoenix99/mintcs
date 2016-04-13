using System;
using System.Collections.Generic;
using System.Linq;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding
{
    public sealed class MegamorphicSiteBinder : CallSiteBinder
    {
        private readonly Dictionary<long, CachedMethod> cache;

        public MegamorphicSiteBinder()
        {
            cache = new Dictionary<long, CachedMethod>();
        }

        internal MegamorphicSiteBinder(Dictionary<long, MethodBinder> cache, CallSite site)
        {
            this.cache = cache.ToDictionary(_ => _.Key, _ => new CachedMethod(_.Value, site));
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

        internal class CachedMethod
        {
            public CachedMethod(MethodBinder binder, CallSite site)
            {
                Call = CompileMethod(Binder = binder, site);
            }

            public MethodBinder Binder { get; }
            public Function     Call   { get; }

            private static Function CompileMethod(MethodBinder binder, CallSite site)
            {
                var instance = Parameter(typeof(iObject), "instance");
                var args = Parameter(typeof(iObject[]), "args");

                // TODO assuming always ParameterKind.Req. change to accept Block, Rest, KeyReq, KeyRest
                var unsplatArgs = Enumerable.Range(0, site.Parameters.Length).Select(i => ArrayIndex(args, Constant(i)));

                var body   = binder.Bind(instance, unsplatArgs);
                var lambda = Lambda<Function>(body, instance, args);
                return lambda.Compile();
            }
        }

        #region Static

        public static readonly Symbol METHOD_MISSING;

        static MegamorphicSiteBinder()
        {
            METHOD_MISSING = new Symbol("method_missing");
        }

        #endregion
    }
}