using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class IfCompiler : CompilerComponentBase
    {
        public IfCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            Push(Node[0]);
            Push(Node[1]);

            if(HasElse())
            {
                Push(Node[2]);
            }
        }

        public override Expression Reduce()
        {
            var condition = Compiler.Pop();
            var trueBody = Compiler.Pop();
            var elseBody = HasElse() ? Pop() : Compiler.NIL;

            condition = CompilerUtils.ToBool(condition);

            if(Node.Value.Type == kUNLESS || Node.Value.Type == kUNLESS_MOD)
            {
                condition = CompilerUtils.Negate(condition);
            }

            return Condition(condition, trueBody, elseBody, typeof(iObject));
        }

        private bool HasElse() => Node.List.Count >= 3 && Node[2].List.Count != 0;
    }
}
