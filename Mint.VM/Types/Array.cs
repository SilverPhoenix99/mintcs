using Mint.Reflection.Parameters.Attributes;
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

        public Array(int count, iObject obj) : this()
        {
            for(int i = 0; i < count; i++)
            {
                list.Add(obj);
            }
        }

        public Array(int count) : this(count, new NilClass())
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

        public iObject Push(params iObject[] elements)
        {
            list.AddRange(elements);
            return this;
        }

        public Array Add(iObject element)
        {
            list.Add(element);
            return this;
        }

        public IEnumerator<iObject> GetEnumerator() => list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

        public override string ToString() => $"[{string.Join(", ", list.Select(_ => _.Inspect()))}]";

        public Array AndAlso(Array other)
        {
            var result = new Array(list);
            result.list.RemoveAll(item => !other.list.Contains(item));
            result.UniqSelf();
            return result;
        }

        public iObject First()
        {
            return this[0];
        }

        public iObject Last()
        {
            return this[-1];
        }

        public Array Clear()
        {
            list.Clear();
            return this;
        }

        public Array CompactSelf()
        {
            list.RemoveAll(item => item == null || item.IsA(Class.NIL));
            return this;
        }

        public Array Compact() => new Array(list).CompactSelf();

        public String Join([Optional] String str = null)
        {
            if(str == null)
            {
                str = new String("");
            }
            return new String(string.Join(str.ToString(), list));
        }

        public Array Replace(Array other)
        {
            if(other == null)
            {
                throw new TypeError("no implicit conversion of nil into Array");
            }
            list.Clear();
            list.AddRange(other.list);
            return this;
        }

        public Array ReverseSelf()
        {
            list.Reverse();
            return this;
        }

        public Array Reverse() => new Array(list).ReverseSelf();

        public Array UniqSelf()
        {
            var buffer = list.Distinct().ToList();
            list.Clear();
            list.AddRange(buffer);
            return this;
        }

        public Array Uniq() => new Array(list).UniqSelf();

        public static Array operator +(Array left, Array right)
        {
            if(left == null || right == null)
            {
                throw new TypeError("no implicit conversion of nil into Array");
            }
            var result = new Array(left.list);
            result.list.AddRange(right);
            return result;
        }

        public static Array operator *(Array left, Fixnum right)
        {
            if(left == null)
            {
                throw new TypeError("undefined method `*' for nil:NilClass");
            }
            var result = new Array();
            for(var i = 0; i < right.Value; i++)
            {
                result.list.AddRange(left);
            }
            return result;
        }

        public static String operator *(Array left, String right) => left.Join(right);

        public static Array operator -(Array left, Array right)
        {
            if (left == null || right == null)
            {
                throw new TypeError("no implicit conversion of nil into Array");
            }
            var result = new Array(left.list);
            result.list.RemoveAll(item => right.Contains(item));
            return result;
        }

        public static explicit operator Array(iObject[] objects) => new Array(objects);
    }
}