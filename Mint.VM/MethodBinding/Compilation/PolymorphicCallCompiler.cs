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

        private static readonly PropertyInfo PROPERTY_EFFECTIVE_CLASS =
            Reflector<iObject>.Property(_ => _.EffectiveClass);

        private static readonly PropertyInfo PROPERTY_ID = Reflector<iObject>.Property(_ => _.Id);

        private static readonly PropertyInfo PROPERTY_VALID = Reflector<Condition>.Property(_ => _.Valid);

        private static readonly MethodInfo METHOD_DEFAULTCALL = Reflector<PolymorphicCallCompiler>.Method(
            _ => _.DefaultCall(default(iObject), default(ArgumentBundle))
        );

        private readonly CallCompilerCache<Expression> cache;
        private readonly ParameterExpression instanceExpression = Parameter(typeof(iObject), "instance");
        private readonly ParameterExpression bundleExpression = Parameter(typeof(ArgumentBundle), "bundle");
        private readonly GotoExpression gotoExpression = Goto(Label(typeof(iObject), "default"), typeof(iObject));

        public PolymorphicCallCompiler(CallSite callSite)
            : base(callSite)
        {
            cache = new CallCompilerCache<Expression>();
        }

        public override CallSite.Stub Compile()
        {
            cache.RemoveInvalidCachedMethods();

            if(IsCacheEmpty())
            {
                return DefaultCall;
            }

            if(IsCacheFull())
            {
                var binderCache = cache.Select(_ => new KeyValuePair<long, MethodBinder>(_.Key, _.Value.Binder));
                CallSite.CallCompiler = new MegamorphicCallCompiler(CallSite, binderCache);
                return CallSite.CallCompiler.Compile();
            }

            var lambda = Lambda<CallSite.Stub>(BuildBodyExpression(), instanceExpression, bundleExpression);
            return lambda.Compile();
        }

        private bool IsCacheEmpty() => cache.Count == 0;

        private bool IsCacheFull() => cache.Count > CACHE_FULL_THRESHOLD;

        private iObject DefaultCall(iObject instance, ArgumentBundle bundle)
        {
            var classId = instance.EffectiveClass.Id;
            var binder = TryFindMethodBinder(instance);
            var cachedMethod = CreateCachedMethod(classId, binder);
            cache.Put(cachedMethod);
            CallSite.BundledCall = Compile();
            return CallSite.BundledCall(instance, bundle);
        }

        private CachedMethod<Expression> CreateCachedMethod(long classId, MethodBinder binder)
        {
            var frame = new CallFrameBinder(CallSite, instanceExpression, bundleExpression);
            var siteExpression = binder.Bind(frame);
            return new CachedMethod<Expression>(classId, binder, siteExpression);
        }

        private Expression BuildBodyExpression()
        {
            var effectiveClassPropertyExpression = instanceExpression.Property(PROPERTY_EFFECTIVE_CLASS);
            var idPropertyExpression = effectiveClassPropertyExpression.Property(PROPERTY_ID);
            var defaultCase = CreateDefaultCase();
            var switchCases = cache.Select(_ => CreateSwitchCases(_.Value));

            return Switch(idPropertyExpression, defaultCase, null, switchCases);
        }

        private SwitchCase CreateSwitchCases(CachedMethod<Expression> method)
        {
            /*
             * case @classId:
             *     return @condition.Valid ? <CachedCall>() : goto default;
             */

            var condition = Constant(method.ClassId);
            var validPropertyExpression = Constant(method.Binder.Condition).Property(PROPERTY_VALID);
            var body = Condition(validPropertyExpression, method.CachedCall, gotoExpression);

            return SwitchCase(body, condition);
        }

        private Expression CreateDefaultCase()
        {
            /*
             * default:
             *     DefaultCall($instance, $bundle);
             */

            return Label(
                gotoExpression.Target,
                Call(
                    Constant(this),
                    METHOD_DEFAULTCALL,
                    instanceExpression,
                    bundleExpression
                )
            );
        }
    }
}
