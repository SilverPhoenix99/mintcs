using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mint
{
    public class Array : BaseObject
    {
        private readonly List<iObject> list;

        public Array() : base(CLASS)
        {
            list = new List<iObject>();

            throw new NotImplementedException();
        }

        public Array(IEnumerable<iObject> objs) : base(CLASS)
        {
            list = new List<iObject>(objs);
        }

        public Array(params iObject[] objs) : this((IEnumerable<iObject>) objs)
        { }

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

        #region Static

        public static readonly Class CLASS;

        static Array()
        {
            CLASS = new Class(new Symbol(MethodBase.GetCurrentMethod().DeclaringType.Name));
            //Object.DefineClass(CLASS);
        }

        #endregion
    }
}