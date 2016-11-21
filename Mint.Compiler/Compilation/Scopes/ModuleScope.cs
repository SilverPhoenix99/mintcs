using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public class ModuleScope : BaseScope
    {
        public override Scope Parent { get; }

        public override ParameterExpression Nesting { get; }

        public ModuleScope(Compiler compiler, Expression self) : base(compiler, self)
        {
            Parent = compiler.CurrentScope;
            Nesting = CreateNestingVariable();
        }
    }
}
