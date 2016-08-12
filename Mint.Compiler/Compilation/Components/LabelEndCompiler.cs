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

            left = ConvertToSymbol(left);
            return CompilerUtils.NewArray(left.Cast<iObject>(), right);
        }

        private static Expression ConvertToSymbol(Expression expression)
        {
            expression = AsConcat(expression);
            expression = expression.StripConversions();
            expression = expression.Cast<String>();
            expression = expression.Cast<string>();
            return Expression.New(CompilerUtils.SYMBOL_CTOR, expression);
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
