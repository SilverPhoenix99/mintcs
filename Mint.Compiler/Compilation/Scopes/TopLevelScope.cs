using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public class TopLevelScope : BaseScope
    {
        public override Scope Parent => this;

        public override CompilerClosure Closure { get; }

        public override Expression Nesting { get; }

        public TopLevelScope(Compiler compiler) : base(compiler)
        {
            Closure = new CompilerClosure();
            Nesting = Array.Expressions.New();
        }
    }
}
