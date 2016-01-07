using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace mint.types
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
                // TODO Method resolution:
                //   1. Methods defined in the object's singleton class (i.e. the object itself)
                //   2. Modules mixed into the singleton class in reverse order of inclusion
                //   3. Methods defined by the object's class
                //   4. Modules included into the object's class in reverse order of inclusion
                //   5. Methods defined by the object's superclass.

                var info = RuntimeType?.GetMethod(name, args.Select(_ => _.RuntimeType).ToArray());

                var instanceExpr = Expression.Convert(Expression, typeof(iObject));

                var propExpr = Expression.Property(instanceExpr, REAL_CLASS_PROPERTY);

                var resultParam = Expression.Parameter(typeof(iObject), "result");

                var callArgs = new Expression[]
                {
                    instanceExpr,
                    Expression.Constant(name),
                    resultParam,
                    Expression.NewArrayInit(typeof(object), args.Select(_ => _.Expression))
                };

                Expression callSite;
                if(info != null)
                {
                    var fallback = Expression.Call(
                        typeof(Object).GetMethod("Convert", new[] { typeof(object) }),
                        fallbackExpr()
                    );

                    callSite = Expression.Block(
                        new[] { resultParam },
                        Expression.Condition(
                            Expression.Call(propExpr, TRY_INVOKE_METHOD, callArgs),
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

                    var fallback = Expression.Throw(
                        Expression.New(
                            NO_METHOD_ERROR_CTOR,
                            Expression.Call(
                                STRING_CONCAT_METHOD,
                                Expression.Constant($"undefined method `{name}' for "),
                                Expression,
                                Expression.Call(Expression, typeof(aObject).GetMethod("InspectInternal"))
                            )
                        )
                    );

                    callSite = Expression.Block(
                        new[] { resultParam },
                        Expression.IfThen(
                            Expression.Not(Expression.Call(propExpr, TRY_INVOKE_METHOD, callArgs)),
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
