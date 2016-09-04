using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.MethodBinding;
using Mint.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation
{
    internal static class CompilerUtils
    {
        private static readonly MethodInfo CONVERT_TO_STRING =
            Reflector.Method(() => System.Convert.ToString(default(object)));

        private static readonly Expression EMPTY_ARRAY = Constant(System.Array.Empty<iObject>());

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
                expr = expr.Cast<iObject>();
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

        public static Expression NewArray(params Expression[] values)
        {
            var array = Array.Expressions.New(Constant(null, typeof(IEnumerable<iObject>)));
            return values.Length == 0
                ? array
                : ListInit(array, values).Cast<iObject>();
        }

        public static Expression Call(
            Expression instance,
            Symbol methodName,
            Visibility visibility,
            params InvocationArgument[] arguments
        )
        {
            var site = new CallSite(methodName, visibility, arguments.Select(_ => _.Kind));
            var argList = arguments.Length == 0
                        ? EMPTY_ARRAY
                        : NewArrayInit(typeof(iObject), arguments.Select(_ => _.Expression));
            return CallSite.Expressions.Call(Constant(site), instance, argList);
        }

        public static Expression StringConcat(Expression first, IEnumerable<Expression> contents)
        {
            contents = contents.Select(ExpressionExtensions.StripConversions);
            contents = new[] { first }.Concat(contents);
            first = contents.Aggregate(StringConcat);

            return first.Cast<iObject>();
        }

        private static Expression StringConcat(Expression left, Expression right)
        {
            right = Expression.Call(CONVERT_TO_STRING, right);
            return String.Expressions.Concat(left, right);
        }
    }
}
