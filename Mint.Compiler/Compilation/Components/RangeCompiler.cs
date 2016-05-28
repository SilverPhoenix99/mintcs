using System.Linq.Expressions;
using Mint.Parse;

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
            var exclude = Expression.Constant(Node.Value.Type == TokenType.kDOT3);
            var range = Expression.New(CompilerUtils.RANGE_CTOR, left, right, exclude);
            return range.Cast<iObject>();
        }
    }
}