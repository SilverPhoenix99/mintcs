using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class StringContentCompiler : CompilerComponentBase
    {
        private string Content => Node.Value.Value;

        public StringContentCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce() => Constant(ReduceContent(), typeof(iObject));

        protected String ReduceContent() => new String(Content);
    }
}