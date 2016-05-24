using Mint.Reflection;
using System.Linq.Expressions;

namespace Mint.Binding.Methods
{
    public interface MethodBinder
    {
        Symbol Name { get; }
        Module Owner { get; }
        Condition Condition { get; }
        Arity Arity { get; }
        Visibility Visibility { get; }

        Expression Bind(InvocationInfo invocationInfo);

        MethodBinder Alias(Symbol newName);

        MethodBinder Duplicate();
    }
}