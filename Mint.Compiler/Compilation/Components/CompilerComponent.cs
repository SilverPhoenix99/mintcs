using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    public interface CompilerComponent
    {
        void Shift();
        Expression Reduce();
    }
}