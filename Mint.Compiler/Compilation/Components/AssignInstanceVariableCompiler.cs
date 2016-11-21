using System.Linq.Expressions;
using Mint.Compilation.Components.Operators;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class AssignInstanceVariableCompiler : AssignVariableCompiler
    {
        private Expression instance;

        private Expression Instance => instance ?? (instance = Compiler.CurrentScope.Self);

        public AssignInstanceVariableCompiler(Compiler compiler, AssignOperator operatorCompiler)
            : base(compiler, operatorCompiler)
        { }

        public override Expression Setter(Expression rightHandSide)
        {
            var variableName = Constant(VariableName);
            return Object.Expressions.InstanceVariableSet(Instance, variableName, rightHandSide);
        }

        protected override Expression CreateGetter()
        {
            var variableName = Constant(VariableName);
            return Object.Expressions.InstanceVariableGet(Instance, variableName);
        }
    }
}
