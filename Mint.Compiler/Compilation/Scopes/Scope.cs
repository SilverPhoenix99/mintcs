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

        ScopeVariable AddNewVariable(Symbol name, ParameterExpression local = null, Expression initialValue = null);

        ScopeVariable AddReferencedVariable(ScopeVariable baseVariable);

        Expression LocalsAdd(Expression variable);

        Expression CompileCallFrameInitialization();
    }
}
