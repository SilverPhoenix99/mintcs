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

        public override Expression Setter(Expression rightHandSide)
        {
            var compilerClosure = Compiler.CurrentScope.Closure;

            if(compilerClosure.IsDefined(VariableName))
            {
                return Assign(Getter, rightHandSide);
            }

            compilerClosure.AddLocal(VariableName);
            return Mint.Closure.Expressions.AddLocal(compilerClosure.Closure, Constant(VariableName), rightHandSide);
        }

        protected override Expression CreateGetter() => Compiler.CurrentScope.Closure.Variable(VariableName);
    }
}
