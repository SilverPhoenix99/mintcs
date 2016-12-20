using System.Linq.Expressions;
using Mint.MethodBinding.Methods;

namespace Mint.Compilation.Scopes.Variables
{
    public class PreInitializedScopeVariable : ScopeVariable
    {
        public Scope Scope { get; }

        public Symbol Name { get; }

        public ParameterExpression Local { get; }

        public PreInitializedScopeVariable(Scope scope, Symbol name, ParameterExpression local)
        {
            Scope = scope;
            Name = name;
        }

        public Expression ValueExpression() => VariableExpression();

        public Expression VariableExpression() => LocalVariable.Expressions.Value(Local);
    }
}