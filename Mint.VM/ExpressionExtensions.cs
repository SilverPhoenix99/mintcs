using System;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint
{
    public static class ExpressionExtensions
    {
        public static Expression Cast<T>(this Expression expression) => Cast(expression, typeof(T));

        public static Expression Cast(this Expression expression, Type type) =>
            expression.Type == type ? expression : Convert(expression, type);

        public static MemberExpression Property(this Expression instance, PropertyInfo property) =>
            Expression.Property(instance, property);

        public static IndexExpression Indexer(
            this Expression instance,
            PropertyInfo property,
            params Expression[] arguments
        ) =>
            Expression.Property(instance, property, arguments);

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
