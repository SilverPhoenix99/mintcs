using System.Linq.Expressions;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal class RelativeNameClassCompiler : ClassCompiler
    {
        protected Ast<Token> LeftNode => Node[0][0];

        protected override Ast<Token> NameNode => Node[0][1];

        protected Expression Container => LeftNode.Accept(Compiler);

        public RelativeNameClassCompiler(Compiler compiler) : base(compiler)
        { }

        protected override Expression GetClass() =>
            Module.Expressions.GetOrCreateClassWithParentCast(Container, Name, CompileSuperclass(), Nesting);
    }
}
