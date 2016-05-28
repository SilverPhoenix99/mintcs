using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Binding;
using Mint.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation
{
    public static class CompilerUtils
    {
        internal static readonly ConstructorInfo STRING_CTOR1 = Reflector.Ctor<String>();
        internal static readonly ConstructorInfo STRING_CTOR2 = Reflector.Ctor<String>(typeof(String));
        private static readonly ConstructorInfo STRING_CTOR3 = Reflector.Ctor<String>(typeof(string));
        internal static readonly ConstructorInfo SYMBOL_CTOR = Reflector.Ctor<Symbol>(typeof(string));
        internal static readonly MethodInfo METHOD_STRING_CONCAT = Reflector<String>.Method(_ => _.Concat(null));
        private static readonly MethodInfo METHOD_OBJECT_TOSTRING = Reflector<object>.Method(_ => _.ToString());
        private static readonly ConstructorInfo ARRAY_CTOR = Reflector.Ctor<Array>(typeof(IEnumerable<iObject>));
        internal static readonly ConstructorInfo RANGE_CTOR =
            Reflector.Ctor<Range>(typeof(iObject), typeof(iObject), typeof(bool));
        private static readonly ConstructorInfo HASH_CTOR = Reflector.Ctor<Hash>();
        private static readonly PropertyInfo MEMBER_HASH_ITEM = Reflector<Hash>.Property(_ => _[default(iObject)]);
        private static readonly PropertyInfo MEMBER_CALLSITE_CALL = Reflector<CallSite>.Property(_ => _.Call);

        private static readonly Expression EMPTY_ARRAY = Constant(new iObject[0]);

        public static Expression ToBool(Expression expr)
        {
            var cnst = expr as ConstantExpression;
            if(cnst != null)
            {
                return Constant(!(cnst.Value is NilClass) && !(cnst.Value is FalseClass));
            }

            // (obj) => obj != null && !(obj is NilClass) && !(obj is FalseClass)

            if(expr.Type != typeof(iObject))
            {
                expr = Convert(expr, typeof(iObject));
            }

            if(expr is ParameterExpression)
            {
                return And(
                    NotEqual(expr, Constant(null)),
                    And(
                        Not(TypeIs(expr, typeof(NilClass))),
                        Not(TypeIs(expr, typeof(FalseClass)))
                    )
                );
            }

            var parm = Variable(typeof(iObject));

            return Block(
                new[] { parm },
                Assign(parm, expr),
                And(
                    NotEqual(parm, Constant(null)),
                    And(
                        Not(TypeIs(parm, typeof(NilClass))),
                        Not(TypeIs(parm, typeof(FalseClass)))
                    )
                )
            );
        }

        public static Expression Negate(Expression expr) =>
            expr.NodeType == ExpressionType.Not ? ((UnaryExpression) expr).Operand : Not(expr);

        public static Expression NewString(Expression argument = null)
        {
            if(argument == null)
            {
                return New(STRING_CTOR1);
            }

            argument = StripConversions(argument);

            if(argument.Type == typeof(String))
            {
                return New(STRING_CTOR2, argument);
            }

            if(argument.Type != typeof(string))
            {
                argument = Convert(argument, typeof(object));
                argument = Call(argument, METHOD_OBJECT_TOSTRING, null);
            }

            return New(STRING_CTOR3, argument);
        }

        public static Expression StripConversions(Expression expression)
        {
            while(expression.NodeType == ExpressionType.Convert)
            {
                expression = ((UnaryExpression) expression).Operand;
            }

            return expression;
        }

        public static Expression NewArray(params Expression[] values)
        {
            var array = New(ARRAY_CTOR, Constant(null, typeof(IEnumerable<iObject>)));
            return values.Length == 0
                ? (Expression) array
                : Convert(ListInit(array, values), typeof(iObject));
        }

        public static Expression Invoke(
            Visibility visibility,
            Expression instance,
            Symbol methodName,
            params InvocationArgument[] arguments
        )
        {
            var site = CallSite.Create(methodName, visibility, arguments.Select(_ => _.Kind));
            var call = Property(Constant(site), MEMBER_CALLSITE_CALL);
            var argList = arguments.Length == 0
                        ? EMPTY_ARRAY
                        : NewArrayInit(typeof(iObject), arguments.Select(_ => _.Expression));
            return Expression.Invoke(call, instance, argList);
        }
    }
}
