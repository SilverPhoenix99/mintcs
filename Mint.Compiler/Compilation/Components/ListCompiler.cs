using Mint.Parse;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class ListCompiler : BaseCompilerComponent
    {
        public ListCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile(Ast<Token> ast)
        {
            return ast.List.Count == 1
                 ? ast[0].Accept(Compiler)
                 : Block(typeof(iObject), ast.Select(_ => _.Accept(Compiler)));
        }
    }
}