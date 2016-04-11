using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mint
{
    public class Array : BaseObject, IEnumerable<iObject>
    {
        private readonly List<iObject> list;

        public Array() : base(CLASS)
        {
            list = new List<iObject>();
        }

        public Array(IEnumerable<iObject> objs) : base(CLASS)
        {
            list = new List<iObject>(objs);
        }

        public Array(params iObject[] objs) : this((IEnumerable<iObject>) objs)
        { }

        public int Count => list.Count;

        public iObject this[iObject index]
        {
            get
            {
                int i;
                //if(index is Fixnum) // TODO !(index is Integer)
                {
                    i = (int) ((Fixnum) index).Value;
                }
                if(i < 0)
                {
                    i += list.Count;
                }
                return 0 <= i && i < list.Count ? list[i] : new NilClass();
            }
            set
            {
                int i;
                //if(index is Fixnum) // TODO !(index is Integer)
                {
                    i = (int) ((Fixnum) index).Value;
                }
                if(i < -list.Count)
                {
                    throw new IndexError($"index {i} too small for array; minimum: -{list.Count}");
                }
                if(i < 0)
                {
                    i += list.Count;
                }
                if(i >= list.Count)
                {
                    list.AddRange(Enumerable.Repeat<iObject>(new NilClass(), i - list.Count + 1));
                }
                list[i] = value;
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

        #region Static

        public static readonly Class CLASS;

        static Array()
        {
            CLASS = ClassBuilder<Array>.Describe()
                .DefMethod("to_s",    _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;
        }

        #endregion
    }
}