using System.Collections.Generic;

namespace Mint
{
    public class Closure
    {
        private readonly Dictionary<Symbol, int> indexes = new Dictionary<Symbol, int>();
        private iObject[] values = new iObject[0];

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
                index = indexes[name] = values.Length;
                System.Array.Resize(ref values, values.Length + 1);
                values[index] = new NilClass();
            }
            return index;
        }
    }
}
