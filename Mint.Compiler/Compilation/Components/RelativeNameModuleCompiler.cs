using System.Linq.Expressions;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal class RelativeNameModuleCompiler : ModuleCompiler
    {
        protected Ast<Token> LeftNode => Node[0][0];

        protected override Ast<Token> NameNode => Node[0][1];

        protected override Expression Container => LeftNode.Accept(Compiler);

        public RelativeNameModuleCompiler(Compiler compiler) : base(compiler)
        { }

        protected override Expression GetModule() => Module.Expressions.GetModuleOrThrow(Container, Name, Nesting);
    }
}
