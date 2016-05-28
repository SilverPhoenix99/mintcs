using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class WordsCompiler : StringCompiler
    {
        public WordsCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            foreach(var child in Node.Where(_ => _.Value.Type != tSPACE))
            {
                Push(child);
            }
        }

        public override Expression Reduce()
        {
            var list = Node.List;
            if(list.Count == 0)
            {
                return CompilerUtils.NewArray();
            }

            var words = new List<Expression>();
            var contents = new List<Expression>();
            foreach(var child in list)
            {
                if(child.Value.Type != tSPACE)
                {
                    contents.Add(Pop());
                    continue;
                }

                var word = CompilerUtils.NewString();
                word = Reduce(word, contents);
                word = word.StripConversions();
                word = Wrap(word);
                word = word.Cast<iObject>();
                words.Add(word);
                contents = new List<Expression>();
            }

            return CompilerUtils.NewArray(words.ToArray());
        }

        protected virtual Expression Wrap(Expression word) => word;
    }
}
