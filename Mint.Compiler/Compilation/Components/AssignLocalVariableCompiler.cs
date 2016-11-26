using Mint.Compilation.Components.Operators;
using Mint.Compilation.Scopes.Variables;
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

        public override Expression Setter(Expression rightHandSide) => Assign(Getter, rightHandSide);

        protected override Expression CreateGetter() => FindOrDefineVariable().ValueExpression();

        private ScopeVariable FindOrDefineVariable() =>
            Compiler.CurrentScope.FindVariable(VariableName)
            ?? Compiler.CurrentScope.AddNewVariable(VariableName);
    }
}
