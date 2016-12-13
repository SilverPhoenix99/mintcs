using System.Linq.Expressions;

namespace Mint.Compilation.Components.Operators
{
    internal class SimpleAssignOperator : AssignOperator
    {
        public Expression Compile(AssignCompiler component) => component.Setter(component.Right);
    }
}
