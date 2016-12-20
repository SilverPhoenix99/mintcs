using Mint.Compilation.Scopes.Variables;
using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public interface Scope
    {
        Scope Parent { get; }

        Expression Nesting { get; }

        Expression CallFrame { get; }

        Expression Instance { get; }

        Expression Module { get; }

        ScopeVariable AddNewVariable(Symbol name, ParameterExpression local = null);

        ScopeVariable AddIndexedVariable(Symbol name);

        ScopeVariable AddReferencedVariable(ScopeVariable baseVariable);

        ScopeVariable AddPreInitializedVariable(Symbol name , ParameterExpression local);

        ScopeVariable FindVariable(Symbol name);

        Expression CompileBody(Expression body);
    }
}
