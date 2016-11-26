using System.Linq.Expressions;

namespace Mint.Compilation.Scopes.Variables
{
    public interface ScopeVariable
    {
        Scope Scope { get; }

        Symbol Name { get; }

        ParameterExpression Local { get; }

        Expression ValueExpression();

        Expression VariableExpression();
    }
}
