using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class HashCompiler : CompilerComponentBase
    {
        public HashCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile() =>
            Node.List.Count == 0 ? Hash.Expressions.New().Cast<iObject>() : CompileElements();

        private Expression CompileElements()
        {
            var hash = Variable(typeof(Hash), "hash");

            var blockExpressions = new List<Expression>
            {
                Assign(hash, Hash.Expressions.New())
            };

            var insertions = GetInsertions(hash);
            blockExpressions.AddRange(insertions);

            blockExpressions.Add(hash);

            return Block(
                typeof(iObject),
                new[] { hash },
                blockExpressions
            );
        }

        private IEnumerable<Expression> GetInsertions(Expression hash) => Node.Select(_ => Insert(hash, _));

        private Expression Insert(Expression hash, Ast<Token> node)
        {
            var element = node.Accept(Compiler);
            var type = node.Value.Type;
            return type == TokenType.kDSTAR ? MergeHash(hash, element) : MergeAssoc(hash, element);
        }

        private static Expression MergeHash(Expression hash, Expression element)
        {
            // h.merge!((Hash) $element)
            element = element.StripConversions().Cast<Hash>();
            return Hash.Expressions.MergeSelf(hash, element).Cast<iObject>();
        }

        private static Expression MergeAssoc(Expression hash, Expression element)
        {
            var array = (ListInitExpression) element.StripConversions();
            var elements = array.Initializers.SelectMany(_ => _.Arguments).ToArray();

            var key = elements[0];
            var value = elements[1];

            // TODO give warning on duplicate keys
            // warning: key <key> is duplicated and overwritten on line <line>

            // hash[$elements[0]] = $elements[1];
            var indexer = Hash.Expressions.Indexer(hash, key);
            return Assign(indexer, value);
        }

    }
}
