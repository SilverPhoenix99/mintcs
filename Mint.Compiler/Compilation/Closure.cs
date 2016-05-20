using System.Collections.Generic;

namespace Mint
{
    public class Closure
    {
        private readonly Dictionary<Symbol, int> indexes = new Dictionary<Symbol, int>();
        private Array values = new Array();

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
    }
}
