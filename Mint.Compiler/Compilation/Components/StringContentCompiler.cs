using System.Linq.Expressions;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class StringContentCompiler : CompilerComponentBase
    {
        public StringContentCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce() => Constant(ReduceContent(), typeof(iObject));

        protected String ReduceContent() => new String(Node.Value.Value);
    }
}