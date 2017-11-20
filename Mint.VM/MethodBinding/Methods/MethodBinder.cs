using System;

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

        Func<iObject> Call { get; }

        MethodBinder Duplicate();

        MethodBinder Duplicate(Symbol newName);
    }
}