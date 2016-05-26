using Mint.Parse;
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

        public override void Shift() => ShiftChildren();

        public override Expression Reduce()
        {
            if(IsSimpleContent(Node))
            {
                return Pop();
            }

            var count = Node.List.Count;
            var contents = Enumerable.Range(0, count).Select(_ => Pop());
            return Reduce(CompilerUtils.NewString(), contents);
        }

        private static bool IsSimpleContent(Ast<Token> node) =>
            node.List.Count == 1 && node[0].Value.Type == tSTRING_CONTENT;

        protected static Expression Reduce(Expression first, IEnumerable<Expression> contents)
        {
            contents = contents.Select(CompilerUtils.StripConversions);
            contents = new[] { first }.Concat(contents);
            first = contents.Aggregate(StringConcat);

            return Convert(first, typeof(iObject));
        }

        private static Expression StringConcat(Expression left, Expression right) =>
            Call(left, CompilerUtils.METHOD_STRING_CONCAT, right);
    }
}