using Mint.Compilation.Components;
using Mint.Parse;

namespace Mint.Compilation.Selectors
{
    public abstract class ComponentSelectorBase : ComponentSelector
    {
        protected Compiler Compiler { get; }
        protected Ast<Token> Node => Compiler.CurrentNode;

        protected ComponentSelectorBase(Compiler compiler)
        {
            Compiler = compiler;
        }

        public abstract CompilerComponent Select();
    }
}
