using System.Linq.Expressions;

namespace Mint.MethodBinding.Methods
{
    public interface MethodBinder
    {
        Symbol Name { get; }

        Module Owner { get; }

        Condition Condition { get; }

        Visibility Visibility { get; }

        Expression Bind(CallFrameBinder frame);

        MethodBinder Duplicate();

        MethodBinder Duplicate(Symbol newName);
    }
}