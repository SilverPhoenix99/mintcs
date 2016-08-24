using Mint.MethodBinding.Methods;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Compilation
{
    public sealed class PolymorphicCallCompiler : BaseCallCompiler
    {
        private const int CACHE_FULL_THRESHOLD = 32;

        private static readonly PropertyInfo PROPERTY_CALCULATEDCLASS =
            Reflector<iObject>.Property(_ => _.EffectiveClass);

        private static readonly PropertyInfo PROPERTY_ID = Reflector<iObject>.Property(_ => _.Id);

        private static readonly PropertyInfo PROPERTY_VALID = Reflector<Condition>.Property(_ => _.Valid);

        private static readonly MethodInfo METHOD_DEFAULTCALL = Reflector<PolymorphicCallCompiler>.Method(
            _ => _.DefaultCall(default(iObject), default(iObject[]))
        );

        private readonly CallCompilerCache<Expression> cache;
        private readonly ParameterExpression instanceExpression = Parameter(typeof(iObject), "instance");
        private readonly ParameterExpression argumentsExpression = Parameter(typeof(iObject[]), "args");

        public PolymorphicCallCompiler(CallSite callSite)
            : base(callSite)
        {
            cache = new CallCompilerCache<Expression>();
        }

        public override Function Compile()
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

            var lambda = Lambda<Function>(BuildBodyExpression(), instanceExpression, argumentsExpression);
            return lambda.Compile();
        }

        private bool IsCacheEmpty() => cache.Count == 0;

        private iObject DefaultCall(iObject instance, iObject[] arguments)
        {
            var classId = instance.EffectiveClass.Id;
            var binder = TryFindMethodBinder(instance);
            var cachedMethod = CreateCachedMethod(classId, binder);
            cache.Put(cachedMethod);
            CallSite.Call = Compile();
            return CallSite.Call(instance, arguments);
        }

        private CachedMethod<Expression> CreateCachedMethod(long classId, MethodBinder binder)
        {
            var invocation = new Invocation(CallSite.CallInfo, instanceExpression, argumentsExpression);
            var siteExpression = binder.Bind(invocation);
            return new CachedMethod<Expression>(classId, binder, siteExpression);
        }

        private bool IsCacheFull() => cache.Count > CACHE_FULL_THRESHOLD;

        private Expression BuildBodyExpression()
        {
            var returnTarget = Label(typeof(iObject), "return");
            var classIdVariableExpression = Variable(typeof(long), "classId");
            var switchCases = cache.Select(_ => CreateSwitchCase(_.Value, returnTarget));

            var calculatedClassPropertyExpression = instanceExpression.Property(PROPERTY_CALCULATEDCLASS);
            var idPropertyExpression = calculatedClassPropertyExpression.Property(PROPERTY_ID);

            return Block(
                typeof(iObject),
                new[] { classIdVariableExpression },

                Assign(classIdVariableExpression, idPropertyExpression),
                Switch(classIdVariableExpression, null, null, switchCases),

                // return DefaultCall(instance, arguments);
                Label(
                    returnTarget,
                    Call(
                        Constant(this),
                        METHOD_DEFAULTCALL,
                        instanceExpression,
                        argumentsExpression
                    )
                )
            );
        }

        private static SwitchCase CreateSwitchCase(CachedMethod<Expression> method, LabelTarget returnTarget)
        {
            var validPropertyExpression = Constant(method.Binder.Condition).Property(PROPERTY_VALID);
            var returnExpression = Return(returnTarget, method.CachedCall, typeof(iObject));

            return SwitchCase(
                IfThen(validPropertyExpression, returnExpression),
                Constant(method.ClassId)
            );
        }
    }
}
