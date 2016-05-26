using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class NotCompiler : CompilerComponentBase
    {
        public NotCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift() => Push(Node[0]);

        public override Expression Reduce()
        {
            var condition = Pop();
            condition = CompilerUtils.ToBool(condition);
            return Condition(condition, Compiler.FALSE, Compiler.TRUE, typeof(iObject));
        }
    }
}