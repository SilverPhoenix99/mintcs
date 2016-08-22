using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components.Operators
{
    internal class OrAssignOperator : AssignOperator
    {
        public Expression Reduce(AssignCompiler component)
        {
            var getter = Variable(typeof(iObject), "getter");
            var setter = component.Setter(component.Right);
            var condition = CompilerUtils.ToBool(getter);

            return Block(
                typeof(iObject),
                new[] { getter },
                Assign(getter, component.Getter),
                MakeCondition(condition, getter, setter)
            );
        }

        protected virtual Expression MakeCondition(Expression condition, Expression left, Expression right)
        {
            return Condition(condition, left, right);
        }
    }
}
