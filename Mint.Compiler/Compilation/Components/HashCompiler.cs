﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

            var blockExpressions = new List<Expression>
            {
                Assign(hash, CompilerUtils.NewHash().StripConversions())
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

        private IEnumerable<Expression> GetInsertions(Expression hash)
        {
            var elements = Enumerable.Range(0, Node.List.Count).Select(_ => Pop()).ToArray();
            var types = Node.List.Select(_ => _.Value.Type);
            return elements.Zip(types, (element, type) => Insert(hash, element, type));
        }

        private static Expression Insert(Expression hash, Expression element, TokenType type)
        {
            return type == TokenType.kDSTAR ? MergeHash(hash, element) : MergeAssoc(hash, element);
        }

        private static Expression MergeHash(Expression hash, Expression element)
        {
            // h.merge!((Hash) $element)
            element = element.StripConversions().Cast<Hash>();
            return Call(hash, MERGE_SELF, element).Cast<iObject>();
        }

        private static Expression MergeAssoc(Expression hash, Expression element)
        {
            var array = (ListInitExpression) element.StripConversions();
            var elements = array.Initializers.SelectMany(_ => _.Arguments).ToArray();

            var key = elements[0];
            var value = elements[1];

            // TODO give warning on duplicat keys
            // warning: key <key> is duplicated and overwritten on line <line>

            // hash[$elements[0]] = $elements[1];
            var indexer = Property(hash, INDEXER, key);
            return Assign(indexer, value);
        }

    }
}
