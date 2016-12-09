using System.Linq.Expressions;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal class LabelEndCompiler : CompilerComponentBase
    {
        private Ast<Token> LeftNode => Node[0];

        private Ast<Token> RightNode => Node[1];

        public LabelEndCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var left = LeftNode.Accept(Compiler);
            var right = RightNode.Accept(Compiler);

            left = ConvertToSymbol(left);
            return CompilerUtils.NewArray(left.Cast<iObject>(), right);
        }

        private static Expression ConvertToSymbol(Expression expression)
        {
            expression = AsConcat(expression);
            expression = expression.StripConversions();
            expression = expression.Cast<String>();
            expression = expression.Cast<string>();
            return Symbol.Expressions.New(expression);
        }

        private static Expression AsConcat(Expression expression)
        {
            var block = expression as BlockExpression;

            return block == null
                ? expression
                : CompilerUtils.StringConcat(String.Expressions.New(), block.Expressions);
        }
    }
}
