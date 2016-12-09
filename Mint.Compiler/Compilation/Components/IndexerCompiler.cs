using Mint.Parse;
using System.Linq;
using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class IndexerCompiler : CompilerComponentBase
    {
        // <left>[*<args>]   =>   <left>.[](*<args>)

        protected virtual Ast<Token> LeftNode => Node[0];
        protected virtual Ast<Token> ArgumentsNode => Node[1];

        public IndexerCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var left = LeftNode.Accept(Compiler);
            var arguments = CompileArguments();

            var visibility = LeftNode.GetVisibility();
            return CompilerUtils.Call(left, Symbol.AREF, visibility, arguments);
        }

        protected InvocationArgument[] CompileArguments()
        {
            return (
                from astArgument in ArgumentsNode
                let argument = astArgument.Accept(Compiler)
                let kind = astArgument.Value.Type.GetArgumentKind()
                select new InvocationArgument(kind, argument)
            ).ToArray();
        }
    }
}
