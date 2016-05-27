using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    public interface CompilerComponent
    {
        Compiler Compiler { get; }

        void Shift();
        Expression Reduce();
    }
}