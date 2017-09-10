using System.Collections.Generic;
using System.Linq.Expressions;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class WordsCompiler : StringCompiler
    {
        public WordsCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            if(Node.List.Count == 0)
            {
                return Array.Expressions.New();
            }

            var words = new List<Expression>();
            var contents = new List<Expression>();
            foreach(var child in Node)
            {
                if(child.Token.Type == tSPACE)
                {
                    var word = CreateWord(contents);
                    words.Add(word);
                    contents = new List<Expression>();
                    continue;
                }

                var content = child.Accept(Compiler);
                contents.Add(content);
            }

            return CompilerUtils.NewArray(words.ToArray());
        }

        private Expression CreateWord(IEnumerable<Expression> contents)
        {
            Expression word = String.Expressions.New();
            word = CompilerUtils.StringConcat(word, contents);
            word = word.StripConversions();
            word = Wrap(word);
            return word.Cast<iObject>();
        }

        protected virtual Expression Wrap(Expression word) => word;
    }
}
