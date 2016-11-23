using Mint.Compilation.Components.Operators;
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
            var compilerClosure = Compiler.CurrentScope.Closure;

            if(!compilerClosure.IsDefined(VariableName))
            {
                compilerClosure.AddVariable(VariableName);
            }

            return Assign(Getter, rightHandSide);
        }

        protected override Expression CreateGetter() =>
            LocalVariable.Expressions.Value(Compiler.CurrentScope.Closure.GetLocal(VariableName));
    }
}
