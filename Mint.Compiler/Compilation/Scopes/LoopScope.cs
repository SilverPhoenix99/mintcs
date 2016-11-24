using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public class LoopScope : BaseScope
    {
        public override Scope Parent { get; }

        public override Expression Nesting => Parent.Nesting;

        protected LoopScope(Compiler compiler, Scope parent) : base(compiler)
        {
            Parent = parent;
        }

        public LoopScope(Compiler compiler) : this(compiler, compiler.CurrentScope)
        { }
    }
}
