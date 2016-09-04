using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint
{
    public class Closure
    {
        private readonly Dictionary<Symbol, int> indexes = new Dictionary<Symbol, int>();
        private readonly Array values = new Array();

        public Closure(Expression self)
        {
            Self = self;
        }

        public Expression Self { get; }

        public iObject this[Symbol sym]
        {
            get { return values[IndexOf(sym)]; }
            set { values[IndexOf(sym)] = value; }
        }

        public iObject this[int index]
        {
            get { return values[index]; }
            set { values[index] = value; }
        }

        public int IndexOf(Symbol name)
        {
            int index;
            if(!indexes.TryGetValue(name, out index))
            {
                indexes[name] = index = values.Count;
                values[index] = new NilClass();
            }
            return index;
        }

        public bool IsDefined(Symbol name) => indexes.ContainsKey(name);

        public Expression Variable(Symbol name) => Expressions.Indexer(Constant(this), Constant(IndexOf(name)));

        public static class Reflection
        {
            public static readonly PropertyInfo Indexer = Reflector<Closure>.Property(_ => _[default(int)]);
        }

        public static class Expressions
        {
            public static IndexExpression Indexer(Expression closure, Expression index) =>
                closure.Indexer(Reflection.Indexer, index);
        }
    }
}
