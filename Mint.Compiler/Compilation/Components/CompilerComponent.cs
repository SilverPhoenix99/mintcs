using Mint.Parse;
using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    public interface CompilerComponent
    {
        Compiler Compiler { get; }

        Expression Compile(Ast<Token> ast);
    }
}