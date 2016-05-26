using System.Linq;
using System.Linq.Expressions;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal class SymbolWordsCompiler : SymbolCompiler
    {
        public SymbolWordsCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile(Ast<Token> ast)
        {
            var lists = from list in GroupWords(ast)
                        where list.Count != 0
                        select Compile(list);

            return Compiler.CreateArray(lists.ToArray());
        }
    }
}
