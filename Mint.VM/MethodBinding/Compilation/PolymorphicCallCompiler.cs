using Mint.MethodBinding.Arguments;
using Mint.MethodBinding.Methods;
using Mint.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Compilation
{
    public sealed class PolymorphicCallCompiler : BaseCallCompiler
    {
        private const int CACHE_FULL_THRESHOLD = 32;

        private CallCompilerCache<Expression> Cache { get; }

        private ParameterExpression Instance { get; } = Parameter(typeof(iObject), "instance");

        private ParameterExpression Bundle { get; } = Parameter(typeof(ArgumentBundle), "bundle");

        private GotoExpression GotoExpression { get; } = Goto(Label("default"), typeof(iObject));

        public PolymorphicCallCompiler(CallSite callSite)
            : base(callSite)
        {
            Cache = new CallCompilerCache<Expression>();
        }

        public override CallSite.Stub Compile()
        {
            Cache.RemoveInvalidCachedMethods();

            if(IsCacheEmpty())
            {
                return DefaultCall;
            }

            if(IsCacheFull())
            {
                return UpgradeCompiler();
            }

            var lambda = Lambda<CallSite.Stub>(BuildBodyExpression(), Instance, Bundle);
            return lambda.Compile();
        }

        private bool IsCacheEmpty() => Cache.Count == 0;

        private bool IsCacheFull() => Cache.Count > CACHE_FULL_THRESHOLD;

        private iObject DefaultCall(iObject instance, ArgumentBundle bundle)
        {
            var classId = instance.EffectiveClass.Id;
            var binder = TryFindMethodBinder(instance);
            var cachedMethod = CreateCachedMethod(classId, binder);
            Cache.Put(cachedMethod);
            CallSite.BundledCall = Compile();
            return CallSite.BundledCall(instance, bundle);
        }

        private CallSite.Stub UpgradeCompiler()
        {
            var binderCache = Cache.Select(_ => new KeyValuePair<long, MethodBinder>(_.Key, _.Value.Binder));
            CallSite.CallCompiler = new MegamorphicCallCompiler(CallSite, binderCache);
            return CallSite.CallCompiler.Compile();
        }

        private CachedMethod<Expression> CreateCachedMethod(long classId, MethodBinder binder)
        {
            var frame = new CallFrameBinder(CallSite, Instance, Bundle);
            var siteExpression = binder.Bind(frame);
            return new CachedMethod<Expression>(classId, binder, siteExpression);
        }

        private Expression BuildBodyExpression()
        {
            var effectiveClassPropertyExpression = Instance.Property(Object.Reflection.EffectiveClass);
            var idPropertyExpression = effectiveClassPropertyExpression.Property(Object.Reflection.Id);
            var defaultCase = CreateDefaultCase();
            var switchCases = Cache.Select(_ => CreateSwitchCases(_.Value));

            return Switch(idPropertyExpression, defaultCase, null, switchCases);
        }

        private SwitchCase CreateSwitchCases(CachedMethod<Expression> method)
        {
            /*
             * case @classId:
             *     return @condition.Valid ? <CachedCall>() : goto default;
             */

            var condition = Constant(method.ClassId);
            var validPropertyExpression = Constant(method.Binder.Condition).Property(Condition.Reflection.Valid);
            var body = Condition(validPropertyExpression, method.CachedCall, GotoExpression);

            return SwitchCase(body, condition);
        }

        private Expression CreateDefaultCase()
        {
            /*
             * default:
             *     DefaultCall($instance, $bundle);
             */

            return Block(
                Label(GotoExpression.Target),
                Expressions.DefaultCall(Constant(this), Instance, Bundle)
            );
        }

        public static class Reflection
        {
            public static readonly MethodInfo DefaultCall = Reflector<PolymorphicCallCompiler>.Method(
                _ => _.DefaultCall(default(iObject), default(ArgumentBundle))
            );
        }

        public static class Expressions
        {
            public static MethodCallExpression DefaultCall(Expression callCompiler,
                                                           Expression instance,
                                                           Expression bundle) =>
                Call(callCompiler, Reflection.DefaultCall, instance, bundle);
        }
    }
}
