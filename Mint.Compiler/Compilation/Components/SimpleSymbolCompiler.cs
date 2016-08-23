using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class SimpleSymbolCompiler : CompilerComponentBase
    {
        private string Identifier => Node[0].Value.Value;

        public SimpleSymbolCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            // Do not shift identifier (child),
            // otherwise the compiler assumes it is a variable or a method call.
        }

        public override Expression Reduce() => Constant(new Symbol(Identifier), typeof(iObject));
    }
}