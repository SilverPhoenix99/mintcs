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
            ScopeVariable variable;

            var currentScope = Compiler.CurrentScope;

            for(var scope = Compiler.CurrentScope; scope != null && scope.Parent != scope; scope = scope.Parent)
            {
                if(!scope.Variables.ContainsKey(VariableName))
                {
                    continue;
                }

                // variable found

                if(scope != Compiler.CurrentScope)
                {
                    // ... in another scope

                    variable = scope.Variables[VariableName];
                    currentScope.AddReferencedVariable(variable);
                }

                return;
            }

            currentScope.AddNewVariable(VariableName);
        }
    }
}
