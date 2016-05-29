using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class LabelEndCompiler : CompilerComponentBase
    {
        public LabelEndCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var left = Pop();
            var right = Pop();

            left = AsConcat(left);

            return CompilerUtils.NewArray(left, right);
        }

        private static Expression AsConcat(Expression expression)
        {
            var block = expression as BlockExpression;

            return block == null
                ? expression
                : CompilerUtils.StringConcat(CompilerUtils.NewString(), block.Expressions);
        }
    }
}
