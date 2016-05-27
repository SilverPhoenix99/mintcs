using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class AssignVariableCompiler : CompilerComponentBase
    {
        // <id> = <right>

        public AssignVariableCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            Push(Node[1]);
        }

        public override Expression Reduce()
        {
            var right = Pop();

            var name = new Symbol(Node[0].Value.Value);
            var local = Compiler.CurrentScope.Closure.Variable(name);
            return Assign(local, right);
        }
    }
}