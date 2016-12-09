using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    class CallScope : BaseScope
    {
        public override Scope Parent { get; }

        public override Expression Nesting => Parent.Nesting;

        public override Expression Instance { get; }

        public CallScope(Compiler compiler, Expression instance) : base(compiler, null)
        {
            Instance = instance;
        }
    }
}
