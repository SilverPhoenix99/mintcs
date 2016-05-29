using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint
{
    public class Closure
    {
        private static readonly PropertyInfo INDEXER = Reflector<Closure>.Property(_ => _[default(int)]);

        private readonly Dictionary<Symbol, int> indexes = new Dictionary<Symbol, int>();
        private readonly Array values = new Array();

        public Closure(iObject self)
        {
            Self = self;
        }

        public iObject Self { get; }

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

        public Expression Variable(Symbol name) => Constant(this).Indexer(INDEXER, Constant(IndexOf(name)));

    }
}
