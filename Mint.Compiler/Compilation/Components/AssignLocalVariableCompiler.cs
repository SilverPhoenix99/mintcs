using Mint.Compilation.Components.Operators;
using Mint.Compilation.Scopes;
using Mint.MethodBinding.Methods;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class AssignLocalVariableCompiler : AssignVariableCompiler
    {
        public AssignLocalVariableCompiler(Compiler compiler, AssignOperator operatorCompiler)
            : base(compiler, operatorCompiler)
        { }

        public override Expression Setter(Expression rightHandSide)
        {
            FindOrDefineVariable();
            return Assign(Getter, rightHandSide);
        }

        protected override Expression CreateGetter() =>
            LocalVariable.Expressions.Value(Compiler.CurrentScope.Variables[VariableName].Local);

        private void FindOrDefineVariable()
        {
            Scope scope;
            ScopeVariable scopeVariable;
            ParameterExpression local;
            Expression initialValue;

            for(scope = Compiler.CurrentScope; scope != null && scope.Parent != scope; scope = scope.Parent)
            {
                if(!scope.Variables.ContainsKey(VariableName))
                {
                    continue;
                }

                if(scope != Compiler.CurrentScope)
                {
                    scopeVariable = scope.Variables[VariableName];
                    scope = Compiler.CurrentScope;
                    local = scopeVariable.Local;
                    initialValue = Compiler.CurrentScope.LocalsIndex(local, scope.Variables.Count);
                    scopeVariable = new ScopeVariable(VariableName, scope.Variables.Count, local, initialValue);
                    scope.Variables.Add(VariableName, scopeVariable);
                }

                return;
            }

            scope = Compiler.CurrentScope;
            local = Expression.Variable(typeof(LocalVariable), VariableName.Name);
            initialValue = scope.LocalsAdd(VariableName, local);
            scopeVariable = new ScopeVariable(VariableName, scope.Variables.Count, local, initialValue);
            scope.Variables.Add(VariableName, scopeVariable);
        }
    }
}
