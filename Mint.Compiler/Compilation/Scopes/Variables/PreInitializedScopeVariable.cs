using System.Linq.Expressions;
using Mint.MethodBinding;

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
            Local = local;
        }

        public Expression ValueExpression() => VariableExpression();

        public Expression VariableExpression() => LocalVariable.Expressions.Value(Local);
    }
}
