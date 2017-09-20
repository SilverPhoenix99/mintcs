using Mint.MethodBinding.Arguments;
using Mint.MethodBinding.Methods;
using Mint.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Compilation
{
    /*
     * Generated Stub:
     *
     * (iObject $instance, ArgumentBundle $bundle) => {
     *
     *     if(@Condition.Valid)
     *     {
     *         <MethodBinder code>;
     *     }
     *     else
     *     {
     *         @CallSite.BundledCall = @CallSite.CallCompiler.Compile();
     *         return @CallSite.BundledCall($instance, $bundle);
     *     }
     * }
     */
    public sealed class PolymorphicCallCompiler : BaseCallCompiler
    {
        private const int CACHE_FULL_THRESHOLD = 32;


        public PolymorphicCallCompiler(CallSite callSite)
            : base(callSite)
        {
            Cache = new CallCompilerCache<Expression>();
        }


        private CallCompilerCache<Expression> Cache { get; }
        private readonly ParameterExpression instanceExpr = Parameter(typeof(iObject), "instance");
        private readonly ParameterExpression bundleExpr = Parameter(typeof(ArgumentBundle), "bundle");
        private readonly GotoExpression gotoExpr = Goto(Label("default"), typeof(iObject));


        public override CallSite.Function Compile()
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

            var lambda = Lambda<CallSite.Function>(BuildBodyExpression(), instanceExpr, bundleExpr);
            return lambda.Compile();
        }


        private bool IsCacheEmpty()
            => Cache.Count == 0;


        private bool IsCacheFull()
            => Cache.Count > CACHE_FULL_THRESHOLD;


        private iObject DefaultCall(iObject instance, ArgumentBundle bundle)
        {
            var binder = TryFindMethodBinder(instance);
            var cachedMethod = CreateCachedMethod(instance, binder);
            Cache.Put(cachedMethod);
            CallSite.BundledCall = Compile();
            return CallSite.BundledCall(instance, bundle);
        }


        private CallSite.Function UpgradeCompiler()
        {
            var binderCache = Cache.Values.Cast<CachedMethod>();
            CallSite.CallCompiler = new MegamorphicCallCompiler(CallSite, binderCache);
            return CallSite.CallCompiler.Compile();
        }


        private CachedMethod<Expression> CreateCachedMethod(iObject instance, MethodBinder binder)
        {
            var classId = instance.EffectiveClass.Id;
            var frame = new CallFrameBinder(CallSite, instance.GetType(), instanceExpr, bundleExpr);
            var siteExpression = binder.Bind(frame);
            return new CachedMethod<Expression>(classId, instance.GetType(), binder, siteExpression);
        }


        private Expression BuildBodyExpression()
        {
            var effectiveClassPropertyExpression = instanceExpr.Property(Object.Reflection.EffectiveClass);
            var idPropertyExpression = effectiveClassPropertyExpression.Property(Object.Reflection.Id);
            var defaultCase = CreateDefaultCase();
            var switchCases = Cache.Values.Select(CreateSwitchCases);

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
            var body = Condition(validPropertyExpression, method.CachedCall, gotoExpr);

            return SwitchCase(body, condition);
        }


        private Expression CreateDefaultCase()
        {
            /*
             * default:
             *     DefaultCall($instance, $bundle);
             */

            return Block(
                Label(gotoExpr.Target),
                Expressions.DefaultCall(Constant(this), instanceExpr, bundleExpr)
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
                                                           Expression bundle)
                => Call(callCompiler, Reflection.DefaultCall, instance, bundle);
        }
    }
}
