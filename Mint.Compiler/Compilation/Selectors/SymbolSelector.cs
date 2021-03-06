﻿using Mint.Compilation.Components;

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
            if(!hasSingleChild)
            {
                return false;
            }

            var firstChild = Node[0];
            var isSimpleChild = firstChild.List.Count == 0;
            return isSimpleChild;
        }
    }
}
