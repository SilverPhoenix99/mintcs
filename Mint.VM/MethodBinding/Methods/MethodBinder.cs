using System.Linq.Expressions;

namespace Mint.MethodBinding.Methods
{
    public interface MethodBinder
    {
        Symbol Name { get; }

        // Module that defines the method (module that has the "def")
        Module Owner { get; }

        // Module that calls the method
        Module Caller { get; }

        Condition Condition { get; }

        Visibility Visibility { get; }

        Expression Bind(CallFrameBinder frame);

        MethodBinder Duplicate();

        MethodBinder Duplicate(Symbol newName);
    }
}