using System.Collections.Generic;
using System.Linq.Expressions;
using Mint.Reflection;

namespace Mint.MethodBinding.Binders
{
    public interface MethodBinder
    {
        Symbol Name { get; }
        Module Owner { get; }
        Condition Condition { get; }
        Arity Arity { get; }
        Visibility Visibility { get; }

        Expression Bind(CallInfo callInfo, Expression instance, Expression arguments);

        MethodBinder Alias(Symbol newName);

        MethodBinder Duplicate();

        IList<ParameterBinder> CreateParameterBinders();
    }
}