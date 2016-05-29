using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Binding;
using Mint.Parse;
using Mint.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class HashCompiler :CompilerComponentBase
    {
        private static readonly MethodInfo MERGE_SELF = Reflector<Hash>.Method(_ => _.MergeSelf(default(Hash)));
        private static readonly PropertyInfo INDEXER = Reflector<Hash>.Property(_ => _[default(iObject)]);

        public HashCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            return Node.List.Count == 0 ? CompilerUtils.NewHash() : ReduceElements();
        }

        private Expression ReduceElements()
        {
            var hash = Variable(typeof(Hash), "hash");

            var elements = Enumerable.Range(0, Node.List.Count).Select(_ => Pop()).ToArray();
            var types = Node.List.Select(_ => _.Value.Type);

            var blockExpressions = new List<Expression>
            {
                Assign(hash, CompilerUtils.NewHash().StripConversions())
            };

            var insertions = elements.Zip(types, (element, type) => Insert(hash, element, type));
            blockExpressions.AddRange(insertions);

            blockExpressions.Add(hash);

            return Block(
                typeof(iObject),
                new[] { hash },
                blockExpressions
            );
        }

        private static Expression Insert(Expression hash, Expression element, TokenType type)
        {
            return type == TokenType.kDSTAR ? MergeHashElement(hash, element) : AddAssoc(hash, element);
        }

        private static Expression MergeHashElement(Expression hash, Expression element)
        {
            // $element = (Hash) (element is Hash ? element : element.to_hash);
            // h.merge!($element)

            var elementToHash = CompilerUtils.Call(element, Symbol.TO_HASH, Visibility.Private);

            var condition = Condition(
                TypeIs(element, typeof(Hash)),
                element,
                elementToHash,
                typeof(iObject)
            ).Cast<Hash>();

            var variable = Variable(typeof(Hash), "element");
            return Block(
                typeof(iObject),
                new[] { variable },
                Assign(variable, condition),
                Call(hash, MERGE_SELF, variable)
            );
        }

        private static Expression AddAssoc(Expression hash, Expression element)
        {
            var array = (ListInitExpression) element.StripConversions();
            var elements = array.Initializers.SelectMany(_ => _.Arguments).ToArray();

            var key = elements[0];
            var value = elements[1];

            // hash[$elements[0]] = $elements[1];
            var indexer = Property(hash, INDEXER, key);
            return Assign(indexer, value);
        }

    }
}
