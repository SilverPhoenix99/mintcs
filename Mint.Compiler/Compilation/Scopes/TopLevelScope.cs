using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public class TopLevelScope : BaseScope
    {
        public override Scope Parent => this;

        public override ParameterExpression Nesting { get; }

        public TopLevelScope(Compiler compiler, Expression self) : base(compiler, self)
        {
            Nesting = CreateNestingVariable();
        }
    }
}
