using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mint.MethodBinding
{
    public interface MethodBinder
    {
        Symbol    Name      { get; }
        Module    Owner     { get; }
        Condition Condition { get; }
        Range     Arity     { get; }

        Expression Bind(CallSite site, Expression instance, IEnumerable<Expression> args);
        Expression Bind(CallSite site, Expression instance, params Expression[] args);

        MethodBinder Duplicate(bool copyValidation);
    }
}