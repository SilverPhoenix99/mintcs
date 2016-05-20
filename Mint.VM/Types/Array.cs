using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mint
{
    public class Array : BaseObject, IEnumerable<iObject>
    {
        private readonly List<iObject> list;

        public Array(IEnumerable<iObject> objs = null) : base(Class.ARRAY)
        {
            list = objs == null ? new List<iObject>() : new List<iObject>(objs);
        }

        public Array(params iObject[] objs) : this((IEnumerable<iObject>) objs)
        { }

        public int Count => list.Count;

        public iObject this[int index]
        {
            get
            {
                if(index < 0)
                {
                    index += list.Count;
                }
                return 0 <= index && index < list.Count ? list[index] : new NilClass();
            }
            set
            {
                if(index < -list.Count)
                {
                    throw new IndexError($"index {index} too small for array; minimum: -{list.Count}");
                }
                if(index < 0)
                {
                    index += list.Count;
                }
                if(index >= list.Count)
                {
                    list.AddRange(Enumerable.Repeat<iObject>(new NilClass(), index - list.Count + 1));
                }
                list[index] = value;
            }
        }

        public iObject this[iObject index]
        {
            get
            {
                int i;
                //if(index is Fixnum) // TODO !(index is Integer)
                {
                    i = (int) (Fixnum) index;
                }
                return this[i];
            }
            set
            {
                int i;
                //if(index is Fixnum) // TODO !(index is Integer)
                {
                    i = (int) (Fixnum) index;
                }
                this[i] = value;
            }
        }

        public iObject Push(params iObject[] elements)
        {
            list.AddRange(elements);
            return this;
        }

        internal void Add(iObject element)
        {
            list.Add(element);
        }

        public IEnumerator<iObject> GetEnumerator() => list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

        public override string ToString() => $"[{string.Join(", ", list.Select(_ => _.Inspect()))}]";

        public static explicit operator Array(iObject[] objects) => new Array(objects);
    }
}