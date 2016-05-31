using Mint.Compilation.Components;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Selectors
{
    internal class SymbolSelector : ComponentSelectorBase
    {
        private CompilerComponent simpleSymbol;
        private CompilerComponent complexSymbol;

        private CompilerComponent SimpleSymbol =>
            simpleSymbol ?? (simpleSymbol = new SimpleSymbolCompiler(Compiler));

        private CompilerComponent ComplexSymbol =>
            complexSymbol ?? (complexSymbol = new ComplexSymbolCompiler(Compiler));

        public SymbolSelector(Compiler compiler) : base(compiler)
        { }

        public override CompilerComponent Select() => IsIdentifierSymbol() ? SimpleSymbol : ComplexSymbol;

        private bool IsIdentifierSymbol()
        {
            var hasSingleChild = Node.List.Count == 1;
            var firstChild = Node[0];
            var type = firstChild.Value.Type;
            return hasSingleChild && (type == tIDENTIFIER || type == tIVAR || type == tCVAR || type == tGVAR);
        }
    }
}
