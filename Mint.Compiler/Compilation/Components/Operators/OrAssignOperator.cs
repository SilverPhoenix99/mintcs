using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components.Operators
{
    internal class OrAssignOperator : OpAssignOperator
    {
        public Expression Reduce(OpAssignCompiler component)
        {
            var getter = Variable(typeof(iObject), "getter");
            var setter = component.Setter(component.Right);

            return Block(
                typeof(iObject),
                new[] { getter },
                Assign(getter, component.Getter),
                Condition(
                    CompilerUtils.ToBool(getter),
                    TrueOption(getter, setter),
                    FalseOption(getter, setter)
                )
            );
        }

        protected virtual Expression TrueOption(Expression left, Expression right) => left;
        protected virtual Expression FalseOption(Expression left, Expression right) => right;
    }
}
