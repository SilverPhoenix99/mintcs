using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public interface Scope
    {
        Scope Parent { get; }

        Expression Nesting { get; }

        Expression CallFrame { get; set; }

        ParameterExpression Locals { get; }

        MemberExpression Instance { get; }

        IDictionary<Symbol, ScopeVariable> Variables { get; }

        Expression LocalsAdd(Symbol variableName, ParameterExpression variable);

        Expression LocalsIndex(ParameterExpression variable, int index);

        Expression CompileCallFrameInitialization();
    }
}
