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
            var scope = Compiler.CurrentScope;

            if(!scope.Variables.ContainsKey(VariableName))
            {
                var variable = new ScopeVariable(VariableName, scope.Variables.Count);
                scope.Variables.Add(VariableName, variable);
            }

            return Assign(Getter, rightHandSide);
        }

        protected override Expression CreateGetter() =>
            LocalVariable.Expressions.Value(Compiler.CurrentScope.Variables[VariableName].Local);
    }
}
