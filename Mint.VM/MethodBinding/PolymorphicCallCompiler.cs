using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding
{
    public sealed class PolymorphicCallCompiler : CallCompiler
    {
        private const int MEGAMORPHIC_THRESHOLD = 32;
        
        private static readonly PropertyInfo PROPERTY_CALCULATEDCLASS =
            Reflector<iObject>.Property(_ => _.CalculatedClass);

        private static readonly PropertyInfo PROPERTY_ID = Reflector<iObject>.Property(_ => _.Id);
        
        private static readonly PropertyInfo PROPERTY_VALID = Reflector<Condition>.Property(_ => _.Valid);
        
        private static readonly MethodInfo METHOD_DEFAULTCALL = Reflector<PolymorphicCallCompiler>.Method(
            _ => _.DefaultCall(default(iObject), default(iObject[]))
        );

        private readonly Dictionary<long, CachedMethod<Expression>> cache =
            new Dictionary<long, CachedMethod<Expression>>();

        private readonly ParameterExpression instanceExpression = Parameter(typeof(iObject), "instance");
        private readonly ParameterExpression argumentsExpression = Parameter(typeof(iObject[]), "args");
        
        public CallSite CallSite { get; }
        
        public PolymorphicCallCompiler(CallSite callSite)
        {
            CallSite = callSite;
        }

        public Function Compile()
        {
            RemoveInvalidCachedMethods();

            if(IsCacheEmpty())
            {
                return DefaultCall;
            }

            if(IsCacheFull())
            {
                CallSite.CallCompiler = CreateUpgradedCallCompiler();
                return CallSite.CallCompiler.Compile();
            }

            var lambda = Lambda<Function>(BuildBodyExpression(), instanceExpression, argumentsExpression);
            return lambda.Compile();
        }
        
        private void RemoveInvalidCachedMethods()
        {
            var invalidKeys = cache.Where(_ => !_.Value.Binder.Condition.Valid).Select(_ => _.Key).ToArray();
            foreach(var key in invalidKeys)
            {
                cache.Remove(key);
            }
        }
        
        private bool IsCacheEmpty() => cache.Count == 0;

        private iObject DefaultCall(iObject instance, iObject[] arguments)
        {
            var classId = instance.CalculatedClass.Id;
            var methodBinder = instance.CalculatedClass.FindMethod(CallSite.CallInfo.MethodName);
            cache[classId] = CreateCachedMethod(classId, methodBinder);
            CallSite.Call = Compile();
            return CallSite.Call(instance, arguments);
        }
        
        private CachedMethod<Expression> CreateCachedMethod(long classId, MethodBinder binder)
        {
            var siteExpression = binder.Bind(CallSite, instanceExpression, argumentsExpression);
            return new CachedMethod<Expression>(classId, binder, siteExpression);
        }
        
        private bool IsCacheFull() => cache.Count > MEGAMORPHIC_THRESHOLD;

        private CallCompiler CreateUpgradedCallCompiler()
        {
            var methodBinderCache = cache.Select(_ => new KeyValuePair<long, MethodBinder>(_.Key, _.Value.Binder));
            return new MegamorphicCallCompiler(CallSite, methodBinderCache);
        }
        
        private Expression BuildBodyExpression()
        {
            var returnTarget = Label(typeof(iObject), "return");
            var classIdVariableExpression = Variable(typeof(long), "classId");
            var switchCases = cache.Select(_ => CreateSwitchCase(_.Value, returnTarget));

            var calculatedClassPropertyExpression = Property(instanceExpression, PROPERTY_CALCULATEDCLASS);
            var idPropertyExpression = Property(calculatedClassPropertyExpression, PROPERTY_ID);

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
            var validPropertyExpression = Property(Constant(method.Binder.Condition), PROPERTY_VALID);
            var returnExpression = Return(returnTarget, method.CachedCall, typeof(iObject));

            return SwitchCase(
                IfThen(validPropertyExpression, returnExpression),
                Constant(method.ClassId)
            );
        }
    }
}
