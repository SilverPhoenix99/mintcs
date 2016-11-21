using Mint.Compilation.Components.Operators;
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

        protected override Expression CreateGetter() => Compiler.CurrentScope.Variable(VariableName);
    }
}
