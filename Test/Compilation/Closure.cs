using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mint
{
    public class Closure
    {
        private readonly Dictionary<Symbol, int> indexes = new Dictionary<Symbol, int>();
        private readonly Array values = new Array();

        public Closure()
        {
            indexes[Symbol.SELF] = 0;
        }

        public iObject this[Symbol sym]
        {
            get { return values[new Fixnum(IndexOf(sym))]; }
            set
            {
                int index;
                if(indexes.TryGetValue(sym, out index))
                {
                    values[new Fixnum(index)] = value;
                }
                else
                {
                    indexes[sym] = values.Count;
                    values.Push(value);
                }
            }
        }

        public iObject this[int index]
        {
            get { return values[new Fixnum(index)]; }
            set { values[new Fixnum(index)] = value; }
        }

        public int IndexOf(Symbol sym)
        {
            int index;
            return indexes.TryGetValue(sym, out index) ? index : values.Count;
        }
    }
}
