using Mint.Reflection;
using System.Linq.Expressions;

namespace Mint.MethodBinding.Methods
{
    public interface MethodBinder
    {
        Symbol Name { get; }
        Module Owner { get; }
        Condition Condition { get; }
        Arity Arity { get; }
        Visibility Visibility { get; }

        Expression Bind(CallFrameBinder frame);

        MethodBinder Duplicate();

        MethodBinder Duplicate(Symbol newName);
    }
}