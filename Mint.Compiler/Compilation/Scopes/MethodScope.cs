using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public class MethodScope : BaseScope
    {
        public override Scope Parent { get; }

        public override ParameterExpression Nesting => Parent.Nesting;

        public MethodScope(Compiler compiler, Expression self) : base(compiler, self)
        {
            Parent = compiler.CurrentScope;
        }
    }
}
