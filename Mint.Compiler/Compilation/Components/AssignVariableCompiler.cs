using System.Linq.Expressions;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class AssignVariableCompiler : CompilerComponentBase
    {
        // <id> = <right>

        private Ast<Token> RightNode => Node[1];
        private string VariableName => Node[0].Value.Value;

        public AssignVariableCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            Push(RightNode);
        }

        public override Expression Reduce()
        {
            var right = Pop();

            var varName = new Symbol(VariableName);
            var local = Compiler.CurrentScope.Closure.Variable(varName);
            return Assign(local, right);
        }
    }
}