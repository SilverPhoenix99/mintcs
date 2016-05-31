using System.Linq.Expressions;
using System.Reflection;
using Mint.Compilation.Components.Operators;
using Mint.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class AssignInstanceVariableCompiler : AssignVariableCompiler
    {
        private Expression instance;
        
        private Expression Instance => instance ?? (instance = Constant(Compiler.CurrentScope.Closure.Self));

        public AssignInstanceVariableCompiler(Compiler compiler, AssignOperator operatorCompiler)
            : base(compiler, operatorCompiler)
        { }

        public override Expression Setter(Expression rightHandSide)
        {
            var variableName = Constant(VariableName);
            return Call(Instance, CompilerUtils.INSTANCE_VARIABLE_SET, variableName, rightHandSide);
        }

        protected override Expression CreateGetter()
        {
            var variableName = Constant(VariableName);
            return Call(Instance, CompilerUtils.INSTANCE_VARIABLE_GET, variableName);
        }
    }
}
