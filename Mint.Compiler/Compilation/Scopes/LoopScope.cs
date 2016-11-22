using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public class LoopScope : BaseScope
    {
        public override Scope Parent { get; }

        public override CompilerClosure Closure => Parent.Closure;

        public override Expression Nesting => Parent.Nesting;

        public LoopScope(Compiler compiler) : base(compiler)
        {
            Parent = compiler.CurrentScope;
        }
    }
}
