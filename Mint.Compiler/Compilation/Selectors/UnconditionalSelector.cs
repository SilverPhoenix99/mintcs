using Mint.Compilation.Components;

namespace Mint.Compilation.Selectors
{
    public class UnconditionalSelector : ComponentSelector
    {
        private readonly CompilerComponent component;

        public UnconditionalSelector(CompilerComponent component)
        {
            this.component = component;
        }

        public CompilerComponent Select() => component;
    }
}
