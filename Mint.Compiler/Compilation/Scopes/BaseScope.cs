using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public abstract class BaseScope : Scope
    {
        public Compiler Compiler { get; }

        public abstract Scope Parent { get; }

        public Expression Self { get; }

        public abstract ParameterExpression Nesting { get; }

        protected BaseScope(Compiler compiler, Expression self)
        {
            Compiler = compiler;
            Self = self;
        }

        protected ParameterExpression CreateNestingVariable() => Expression.Variable(typeof(IList<Module>), "nesting");
    }
}
