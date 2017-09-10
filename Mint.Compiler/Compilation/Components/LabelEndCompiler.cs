using System.Linq.Expressions;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal class LabelEndCompiler : CompilerComponentBase
    {
        private SyntaxNode LeftNode => Node[0];

        private SyntaxNode RightNode => Node[1];

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

        private static Expression AsConcat(Expression expression) =>
            expression is BlockExpression block
                ? CompilerUtils.StringConcat(String.Expressions.New(), block.Expressions)
                : expression;
    }
}
