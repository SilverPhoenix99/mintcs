using Mint.MethodBinding.Arguments;
using System.Linq.Expressions;

namespace Mint.Compilation.Components.Operators
{
    internal class GenericOpAssignOperator : AssignOperator
    {
        public Expression Compile(AssignCompiler component)
        {
            var instance = component.Getter;
            var methodName = component.Operator;
            var argument = new InvocationArgument(ArgumentKind.Simple, component.Right);
            var call = CompilerUtils.Call(instance, methodName, component.Visibility, argument);
            return component.Setter(call);
        }
    }
}
