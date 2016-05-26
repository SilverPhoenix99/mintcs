using System.Linq;
using System.Linq.Expressions;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class WordsCompiler : StringCompiler
    {
        public WordsCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile(Ast<Token> ast)
        {
            var lists = from list in GroupWords(ast)
                        where list.Count != 0
                        select Compile(New(STRING_CTOR1), list);

            return Compiler.CreateArray(lists.ToArray());
        }
    }
}
