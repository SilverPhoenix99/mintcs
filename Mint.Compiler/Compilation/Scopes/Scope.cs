using Mint.Compilation.Scopes.Variables;
using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public interface Scope
    {
        Scope Parent { get; }

        Expression Nesting { get; }

        Expression CallFrame { get; }

        MemberExpression Instance { get; }

        ScopeVariable AddNewVariable(Symbol name, ParameterExpression local = null);

        ScopeVariable AddIndexedVariable(Symbol name);

        ScopeVariable AddReferencedVariable(ScopeVariable baseVariable);

        ScopeVariable FindVariable(Symbol name);

        Expression CompileBody(Expression body);
    }
}
