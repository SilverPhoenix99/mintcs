using System.Linq.Expressions;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    public abstract class CompilerComponentBase : CompilerComponent
    {
        protected Compiler Compiler { get; }

        protected Ast<Token> Node => Compiler.CurrentNode;

        protected CompilerComponentBase(Compiler compiler)
        {
            Compiler = compiler;
        }
        
        public abstract Expression Compile();
    }
}