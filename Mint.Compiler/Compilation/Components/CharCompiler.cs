using Mint.Parse;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class CharCompiler : StringCompiler
    {
        public CharCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile(Ast<Token> ast)
        {
            var first = New(STRING_CTOR2, CompileContent(ast));

            return ast.List.Count == 0
                ? Convert(first, typeof(iObject))
                : Compile(first, ast);
        }
    }
}