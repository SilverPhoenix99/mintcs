using System.Linq.Expressions;

namespace Mint.Compilation.Components.Operators
{
    internal class AndAssignOperator : OrAssignOperator
    {
        protected override Expression TrueOption(Expression left, Expression right) => right;
        protected override Expression FalseOption(Expression left, Expression right) => left;
    }
}
