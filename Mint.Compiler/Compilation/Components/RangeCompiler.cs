using System.Linq.Expressions;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class RangeCompiler : CompilerComponentBase
    {
        public RangeCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var left = Pop();
            var right = Pop();
            var exclude = Constant(Node.Value.Type == TokenType.kDOT3);
            var range = New(CompilerUtils.RANGE_CTOR, left, right, exclude);
            return range.Cast<iObject>();
        }
    }
}