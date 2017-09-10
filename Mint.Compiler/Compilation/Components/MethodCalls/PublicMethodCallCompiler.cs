using System.Linq.Expressions;
using Mint.MethodBinding;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal class PublicMethodCallCompiler : PrivateMethodCallCompiler
    {
        private SyntaxNode LeftNode => Node[0];

        public PublicMethodCallCompiler(Compiler compiler) : base(compiler)
        { }

        protected override Expression GetLeftExpression() => LeftNode.Accept(Compiler);

        protected override Visibility GetVisibility() => LeftNode.GetVisibility();
    }
}
