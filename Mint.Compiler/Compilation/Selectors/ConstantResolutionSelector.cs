using Mint.Compilation.Components;
using Mint.Parse;

namespace Mint.Compilation.Selectors
{
    internal class ConstantResolutionSelector : ComponentSelectorBase
    {
        private CompilerComponent constantResolutionCompiler;
        private CompilerComponent methodCallCompiler;

        private Ast<Token> Right => Node[1];

        private CompilerComponent ConstantResolutionCompiler =>
            constantResolutionCompiler ?? (constantResolutionCompiler = new ConstantResolutionCompiler(Compiler));

        private CompilerComponent MethodCallCompiler =>
            methodCallCompiler ?? (methodCallCompiler = new PublicMethodCallCompiler(Compiler));

        public ConstantResolutionSelector(Compiler compiler) : base(compiler)
        { }

        public override CompilerComponent Select() =>
            Right.Value.Type == TokenType.tCONSTANT
            && Node.List.Count == 2 // meaning, no arguments
                ? ConstantResolutionCompiler
                : MethodCallCompiler;
    }
}
