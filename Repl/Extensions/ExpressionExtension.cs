using System.Linq.Expressions;

namespace Mint.Extensions
{
    static class ExpressionExtension
    {
        public static bool IsConstant(this Expression expr)
        {
            return expr.NodeType == ExpressionType.Constant;
        }

        public static bool IsConstant<T>(this Expression expr)
        {
            return expr.IsConstant() && ((ConstantExpression) expr).Type == typeof(T);
        }
    }
}
