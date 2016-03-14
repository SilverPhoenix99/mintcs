using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.Types
{
    partial class Object
    {
        internal class Meta : DynamicMetaObject
        {
            private static readonly MethodInfo STRING_CONCAT_METHOD = typeof(string)
                .GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });

            private static readonly ConstructorInfo NO_METHOD_ERROR_CTOR = typeof(NoMethodError).GetConstructor(new[] { typeof(string) });

            private static readonly PropertyInfo REAL_CLASS_PROPERTY = typeof(iObject).GetProperty("RealClass");

            private static readonly MethodInfo TRY_INVOKE_METHOD = typeof(Class).GetMethod("TryInvokeMethod",
                new[] { typeof(iObject), typeof(string), typeof(iObject).MakeByRefType(), typeof(object[]) });


            internal Meta(Expression expression, iObject value)
                : base(expression, BindingRestrictions.Empty, value)
            { }


            public new iObject Value => (iObject) ((DynamicMetaObject) this).Value;
                        

            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            {
                Func<Expression> fallbackExpr = () => binder.FallbackInvokeMember(this, args).Expression;
                return CompileMethodInvoke(binder.Name, args, fallbackExpr);
            }


            public override DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg)
            {
                Func<Expression> fallbackExpr = () => binder.FallbackBinaryOperation(this, arg).Expression;
                return CompileMethodInvoke(Operator(binder.Operation), new[] { arg }, fallbackExpr);
            }


            private DynamicMetaObject CompileMethodInvoke(string name, DynamicMetaObject[] args, Func<Expression> fallbackExpr)
            {
                var method = Value.RealClass.FindMethod(name);

                if(method != null)
                {
                    // TODO
                    throw new NotImplementedException();
                }

                var info = RuntimeType?.GetMethod(name, args.Select(_ => _.RuntimeType).ToArray());

                var instanceExpr = Convert(Expression, typeof(iObject));

                var propExpr = Property(instanceExpr, REAL_CLASS_PROPERTY);

                var resultParam = Parameter(typeof(iObject), "result");

                var callArgs = new Expression[]
                {
                    instanceExpr,
                    Constant(name),
                    resultParam,
                    NewArrayInit(typeof(object), args.Select(_ => _.Expression))
                };

                Expression callSite;
                if(info != null)
                {
                    var fallback = Call(
                        typeof(Object).GetMethod("Box", new[] { typeof(object) }),
                        fallbackExpr()
                    );

                    callSite = Block(
                        new[] { resultParam },
                        Condition(
                            Call(propExpr, TRY_INVOKE_METHOD, callArgs),
                            resultParam,
                            fallback,
                            typeof(iObject)
                        )
                    );
                }
                else
                {
                    // expression:
                    //     throw new NoMethodError("undefined method `{0}' for " + <value>.InternalInspect())

                    var fallback = Throw(
                        New(
                            NO_METHOD_ERROR_CTOR,
                            Call(
                                STRING_CONCAT_METHOD,
                                Constant($"undefined method `{name}' for "),
                                Expression,
                                Call(Expression, typeof(aObject).GetMethod("InspectInternal"))
                            )
                        )
                    );

                    callSite = Block(
                        new[] { resultParam },
                        IfThen(
                            Not(Call(propExpr, TRY_INVOKE_METHOD, callArgs)),
                            fallback
                        ),
                        resultParam
                    );
                }

                var restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);

                return new DynamicMetaObject(callSite, restrictions);
            }
        }
    }
}
