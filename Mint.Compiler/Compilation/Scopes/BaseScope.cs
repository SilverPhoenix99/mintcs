using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public abstract class BaseScope : Scope
    {
        public Compiler Compiler { get; }

        public abstract Scope Parent { get; }

        public abstract CompilerClosure Closure { get; }

        public abstract Expression Nesting { get; }

        protected BaseScope(Compiler compiler)
        {
            Compiler = compiler;
        }
    }
}
