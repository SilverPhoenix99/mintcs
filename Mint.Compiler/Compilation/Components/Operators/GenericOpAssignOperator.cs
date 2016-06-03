using Mint.Binding.Arguments;
using System.Linq.Expressions;

namespace Mint.Compilation.Components.Operators
{
    internal class GenericOpAssignOperator : AssignOperator
    {
        public Expression Reduce(AssignCompiler component)
        {
            var argument = new InvocationArgument(ArgumentKind.Simple, component.Right);
            var call = CompilerUtils.Call(component.Getter, component.Operator, component.Visibility, argument);
            return component.Setter(call);
        }
    }
}