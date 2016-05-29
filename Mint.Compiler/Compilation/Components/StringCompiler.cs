using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class StringCompiler : StringContentCompiler
    {
        public StringCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            if(IsSimpleContent())
            {
                return Pop();
            }

            var count = Node.List.Count;
            var contents = Enumerable.Range(0, count).Select(_ => Pop());
            return CompilerUtils.StringConcat(CompilerUtils.NewString(), contents);
        }

        private bool IsSimpleContent()
        {
            var hasSingleChild = Node.List.Count == 1;
            var firstChild = Node[0];
            return hasSingleChild && firstChild.Value.Type == tSTRING_CONTENT;
        }
    }
}