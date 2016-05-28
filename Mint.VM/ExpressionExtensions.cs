using System;
using System.Linq.Expressions;

namespace Mint
{
    public static class ExpressionExtensions
    {
        public static Expression Cast<T>(this Expression expression) => Cast(expression, typeof(T));

        public static Expression Cast(this Expression expression, Type type) =>
            Expression.Convert(expression, type);

        public static Expression StripConversions(this Expression expression)
        {
            while(expression.NodeType == ExpressionType.Convert)
            {
                expression = ((UnaryExpression) expression).Operand;
            }

            return expression;
        }
    }
}
