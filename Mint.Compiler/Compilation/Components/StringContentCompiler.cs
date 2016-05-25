using Mint.Parse;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class StringContentCompiler : BaseCompilerComponent
    {
        public StringContentCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile(Ast<Token> ast) => Constant(new String(ast.Value.Value));
    }
}