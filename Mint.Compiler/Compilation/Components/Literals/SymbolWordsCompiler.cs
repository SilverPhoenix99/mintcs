using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class SymbolWordsCompiler : WordsCompiler
    {
        public SymbolWordsCompiler(Compiler compiler) : base(compiler)
        { }

        protected override Expression Wrap(Expression word) => Symbol.Expressions.New(word.Cast<string>());
    }
}
