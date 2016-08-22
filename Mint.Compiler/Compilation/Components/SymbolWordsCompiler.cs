using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class SymbolWordsCompiler : WordsCompiler
    {
        public SymbolWordsCompiler(Compiler compiler) : base(compiler)
        { }

        protected override Expression Wrap(Expression word) => New(CompilerUtils.SYMBOL_CTOR, word.Cast<string>());
    }
}
