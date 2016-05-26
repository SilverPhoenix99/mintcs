using System.Linq.Expressions;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class StringContentCompiler : CompilerComponentBase
    {
        public StringContentCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce() => Constant(ReduceContent(Node), typeof(iObject));
 
        protected static String ReduceContent(Ast<Token> node) => new String(node.Value.Value);
    }
}