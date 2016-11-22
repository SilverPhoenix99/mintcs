using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mint
{
    public class CompilerClosure
    {
        private readonly List<Symbol> locals;

        public Expression Closure { get; set; }

        public MemberExpression Self => Mint.Closure.Expressions.Self(Closure);

        public MemberExpression Parent => Mint.Closure.Expressions.Parent(Closure);

        public CompilerClosure()
        {
            Closure = Expression.Variable(typeof(Closure), "closure");
            locals = new List<Symbol>();
            AddLocal(Symbol.SELF);
        }

        public int AddLocal(Symbol name)
        {
            locals.Add(name);
            return locals.Count - 1;
        }

        public Expression Variable(Symbol name)
        {
            var index = locals.IndexOf(name);
            return Mint.Closure.Expressions.Indexer(Closure, Expression.Constant(index));
        }

        public bool IsDefined(Symbol name) => locals.IndexOf(name) > -1;
    }
}
