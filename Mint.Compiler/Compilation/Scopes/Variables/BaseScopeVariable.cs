using Mint.MethodBinding.Methods;
using System.Linq.Expressions;

namespace Mint.Compilation.Scopes.Variables
{
    public abstract class BaseScopeVariable : ScopeVariable
    {
        private bool isInitialized;

        public Scope Scope { get; }

        public Symbol Name { get; }

        public ParameterExpression Local { get; }


        protected BaseScopeVariable(Scope scope, Symbol name, ParameterExpression local)
        {
            Scope = scope;
            Name = name;
            Local = local;
            isInitialized = false;
        }

        public Expression ValueExpression() => LocalVariable.Expressions.Value(VariableExpression());

        public Expression VariableExpression()
        {
            if(isInitialized)
            {
                return Local;
            }

            isInitialized = true;
            return CompileInitialization();
        }

        protected abstract Expression CompileInitialization();
    }
}
