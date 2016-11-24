using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public class TopLevelScope : BaseScope
    {
        public override Scope Parent => this;

        public override Expression Nesting => Array.Expressions.New();

        public TopLevelScope(Compiler compiler) : base(compiler)
        { }
    }
}
