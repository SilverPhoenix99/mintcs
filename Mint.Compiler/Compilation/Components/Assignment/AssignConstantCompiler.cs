using Mint.Compilation.Components.Operators;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class AssignConstantCompiler : AssignVariableCompiler
    {
        public AssignConstantCompiler(Compiler compiler, AssignOperator operatorCompiler)
            : base(compiler, operatorCompiler)
        { }

        public override Expression Setter(Expression rightHandSide) =>
            Module.Expressions.SetConstant(Compiler.CurrentScope.Module, Constant(VariableName), rightHandSide);

        protected override Expression CreateGetter() =>
            Module.Expressions.GetConstant(Compiler.CurrentScope.Module, Constant(VariableName));
    }
}
