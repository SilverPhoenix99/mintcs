using System;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;
using static System.Reflection.BindingFlags;

namespace Mint
{
    public static class ExpressionExtensions
    {
        // ReSharper disable once PossibleNullReferenceException
        private static readonly MethodInfo DEBUGVIEW_INFO =
            typeof(Expression).GetProperty("DebugView", Instance | NonPublic).GetMethod;


        public static Expression Cast<T>(this Expression expression)
            => Cast(expression, typeof(T));


        public static Expression Cast(this Expression expression, Type type)
            => expression.Type == type ? expression : Convert(expression, type);


        public static MemberExpression Property(this Expression instance, PropertyInfo property)
            => Expression.Property(instance, property);


        public static IndexExpression Indexer(this Expression instance,
                                              PropertyInfo property,
                                              params Expression[] arguments)
            => Expression.Property(instance, property, arguments);


        public static Expression StripConversions(this Expression expression)
        {
            while(expression.NodeType == ExpressionType.Convert)
            {
                expression = ((UnaryExpression) expression).Operand;
            }

            return expression;
        }


        public static string Inspect(this Expression expr)
            => (string) DEBUGVIEW_INFO.Invoke(expr, System.Array.Empty<object>());
    }
}
