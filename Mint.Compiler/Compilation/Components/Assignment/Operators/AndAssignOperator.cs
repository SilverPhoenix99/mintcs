using System.Linq.Expressions;

namespace Mint.Compilation.Components.Operators
{
    internal class AndAssignOperator : OrAssignOperator
    {
        protected override Expression MakeCondition(Expression condition, Expression left, Expression right)
        {
            return Expression.Condition(condition, right, left);
        }
    }
}
