using Mint.Compilation.Components;
using Mint.Parse;

namespace Mint.Compilation.Selectors
{
    internal class CaseWhenSelector : ComponentSelectorBase
    {
        private CompilerComponent caseWhenWithValueCompiler;
        private CompilerComponent caseWhenCompiler;

        private Ast<Token> ValueNode => Node[0];

        private bool HasValue => ValueNode.Value != null || ValueNode.List.Count > 0;

        private CompilerComponent CaseWhenWithValueCompiler =>
            caseWhenWithValueCompiler ?? (caseWhenWithValueCompiler = new CaseWhenWithValueCompiler(Compiler));

        private CompilerComponent CaseWhenCompiler =>
            caseWhenCompiler ?? (caseWhenCompiler = new CaseWhenCompiler(Compiler));

        public CaseWhenSelector(Compiler compiler) : base(compiler)
        { }

        public override CompilerComponent Select() => HasValue ? CaseWhenWithValueCompiler : CaseWhenCompiler;
    }
}