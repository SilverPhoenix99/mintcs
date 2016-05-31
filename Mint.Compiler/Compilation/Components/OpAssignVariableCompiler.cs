using Mint.Compilation.Components.Operators;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class OpAssignVariableCompiler : OpAssignCompiler
    {
        private Expression getter;

        public override Expression Getter => getter;

        private string VariableName => Node[0].Value.Value;

        public OpAssignVariableCompiler(Compiler compiler, OpAssignOperator operatorCompiler)
            : base(compiler, operatorCompiler)
        { }

        public override void Shift()
        {
            Push(RightNode);
        }

        public override Expression Reduce()
        {
            Right = Pop();

            var varName = new Symbol(VariableName);
            getter = Compiler.CurrentScope.Closure.Variable(varName);

            return base.Reduce();
        }

        public override Expression Setter(Expression rightHandSide) => Assign(Getter, rightHandSide);
    }
}
