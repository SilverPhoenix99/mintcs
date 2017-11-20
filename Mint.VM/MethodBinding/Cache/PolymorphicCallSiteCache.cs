using System;
using System.Collections.Generic;
using System.Linq;
using Mint.MethodBinding.Methods;
using Mint.Reflection;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Cache
{
    /*
     * Generated Stub:
     *
     * () => {
     *     switch(CallFrame.Current.Instance.EffectiveClass.Id)
     *     {
     *         case @ClassId_Long_1: return CallWrapper(@MethodBinderRef_1);
     *         case @ClassId_Long_2: return CallWrapper(@MethodBinderRef_2);
     *         default: return DefaultCall();
     *     }
     * }
     */
    public sealed class PolymorphicCallSiteCache : MegamorphicCallSiteCache
    {
        public const int MAX_CACHE_THRESHOLD = 32;

        private Func<iObject> cachedCall;

        public PolymorphicCallSiteCache(CallSite callSite, IDictionary<long, WeakReference<MethodBinder>> cache = null)
            : base(callSite, cache)
        {
            cachedCall = DefaultCall;
        }

        public override iObject Call() => cachedCall();

        private iObject DefaultCall() => (cachedCall = Compile())();

        private Func<iObject> Compile()
        {
            RemoveInvalidCachedMethods();

            var classId = CallFrame.Current.Instance.EffectiveClass.Id;
            var binder = TryFindMethodBinder();
            cache[classId] = new WeakReference<MethodBinder>(binder);

            if(IsCacheFull)
            {
                UpgradeSiteCache();
                return () => CallSite.CallCache.Call();
            }

            var body = Bind();
            var lambda = Lambda<Func<iObject>>(body);
            return lambda.Compile();
        }

        private bool IsCacheFull => cache.Count > MAX_CACHE_THRESHOLD;
        
        private void UpgradeSiteCache() => CallSite.CallCache = new MegamorphicCallSiteCache(CallSite, cache);
        
        private Expression Bind()
        {
            var classIdExpression = CallFrame.Expressions.Instance(CallFrame.Expressions.Current())
                .Property(Object.Reflection.EffectiveClass)
                .Property(Object.Reflection.Id);

            // default: DefaultCall();
            var defaultCase = Expressions.DefaultCall(Constant(this));

            /*
             * case @classId:
             *     return CallWrapper(@binderRef);
             */

            var switchCases =
                from methodBinder in cache
                select SwitchCase(
                    Expressions.CallWrapper(Constant(this), Constant(methodBinder.Value)),
                    Constant(methodBinder.Key)
                );

            return Switch(classIdExpression, defaultCase, null, switchCases);
        }

        private iObject CallWrapper(WeakReference<MethodBinder> binderRef) =>
            binderRef.TryGetTarget(out var binder) && binder.Condition.Valid
                ? binder.Call()
                : DefaultCall();

        public static class Reflection
        {
            public static readonly MethodInfo CallWrapper = Reflector<PolymorphicCallSiteCache>.Method(
                _ => _.CallWrapper(default)
            );

            public static readonly MethodInfo DefaultCall = Reflector<PolymorphicCallSiteCache>.Method(
                _ => _.DefaultCall()
            );
        }

        public static class Expressions
        {
            public static MethodCallExpression CallWrapper(Expression siteCache, Expression methodBinderRef) =>
                Expression.Call(siteCache, Reflection.CallWrapper, methodBinderRef);

            public static MethodCallExpression DefaultCall(Expression siteCache) =>
                Expression.Call(siteCache, Reflection.DefaultCall);
        }
    }
}
