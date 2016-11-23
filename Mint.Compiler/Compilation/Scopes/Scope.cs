using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public interface Scope
    {
        Scope Parent { get; }

        CompilerClosure Closure { get; }

        Expression Nesting { get; }
    }
}
