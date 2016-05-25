using Mint.Parse;
using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal abstract class BaseCompilerComponent : CompilerComponent
    {
        public Compiler Compiler { get; }

        public abstract Expression Compile(Ast<Token> ast);

        protected BaseCompilerComponent(Compiler compiler)
        {
            Compiler = compiler;
        }
    }
}