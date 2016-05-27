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

        private bool IsIdentifierSymbol() =>
            Compiler.CurrentNode.List.Count == 1 && Compiler.CurrentNode[0].Value.Type == tIDENTIFIER;
    }
}
