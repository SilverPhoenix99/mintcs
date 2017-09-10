using Mint.Compilation.Components;
using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Selectors
{
    internal class WhileModSelector : ComponentSelectorBase
    {
        private CompilerComponent whileCompiler;
        private CompilerComponent postfixWhileCompiler;

        private SyntaxNode BodyNode => Node[1];

        private CompilerComponent WhileCompiler => whileCompiler ?? (whileCompiler = new WhileCompiler(Compiler));

        private CompilerComponent PostfixWhileCompiler =>
            postfixWhileCompiler ?? (postfixWhileCompiler = new PosfixWhileCompiler(Compiler));

        public WhileModSelector(Compiler compiler) : base(compiler)
        { }

        public override CompilerComponent Select()
        {
            return BodyNode?.Token.Type == kBEGIN ? PostfixWhileCompiler : WhileCompiler;
        }
    }
}
