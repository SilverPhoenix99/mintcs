using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class SimpleSymbolCompiler : CompilerComponentBase
    {
        public SimpleSymbolCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        { }
        
        public override Expression Reduce()
        {
            return Constant(new Symbol(Node[0].Value.Value), typeof(iObject));
        }
    }
}