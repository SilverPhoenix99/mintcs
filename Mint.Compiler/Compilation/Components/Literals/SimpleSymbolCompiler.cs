using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class SimpleSymbolCompiler : CompilerComponentBase
    {
        private string Identifier => Node[0].Token.Text;

        public SimpleSymbolCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile() => Constant(new Symbol(Identifier), typeof(iObject));
    }
}