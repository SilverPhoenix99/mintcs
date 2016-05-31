using Mint.Reflection.Parameters.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mint
{
    public class Array : BaseObject, IEnumerable<iObject>
    {
        private readonly List<iObject> list;

        // TODO count(iObject); count &block
        public int Count => list.Count;

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

        public iObject this[int index, int count]
        {
            get
            {
                if(count < 1 || index > Count)
                {
                    return null;
                }
                if(index == Count)
                {
                    return new Array();
                }

                if(index < 0)
                {
                    index += list.Count;
                }

                if(index + count > Count)
                {
                    count = Count - index;
                }
                return new Array(list.GetRange(index, count));
            }
            set
            {
                // TODO setter
                throw new System.NotImplementedException();
            }
        }

        // TODO
        public iObject this[Range range]
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
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

        public iObject First() => this[0];

        public Array First(int count)
        {
            if(count > Count)
            {
                count = Count;
            }
            return new Array(list.GetRange(0, count));
        }

        public iObject Last() => this[-1];

        public Array Last(int count)
        {
            if (count > Count)
            {
                count = Count;
            }
            return new Array(list.GetRange(Count - count, count));
        }

        public Array AndAlso(Array other)
        {
            var result = new Array(list);
            result.list.RemoveAll(item => !other.list.Contains(item));
            result.UniqSelf();
            return result;
        }

        public Array Clear()
        {
            list.Clear();
            return this;
        }

        public Array CompactSelf()
        {
            var count = list.Count;
            list.RemoveAll(item => NilClass.IsNil(item));

            return list.Count == count ? null : this;
        }

        public Array Compact()
        {
            var array = new Array(list);
            array.CompactSelf();
            return array;
        }

        // TODO &block
        public iObject Delete(iObject obj)
        {
            iObject result = null;
            while(list.Contains(obj))
            {
                result = list.Find(item => item.Equals(obj));
                list.Remove(obj);
            }
            return result;
        }

        public iObject DeleteAt(int index)
        {
            if (index >= Count || index < -Count)
            {
                return null;
            }
            var result = this[index];
            if(index < 0)
            {
                index += list.Count;
            }
            list.RemoveAt(index);
            return result;
        }

        public Array Drop(int count)
        {
            if(count < 0)
            {
                throw new ArgumentError();
            }
            var result = new Array(list);
            result.list.RemoveRange(0, count);
            return result;
        }

        public bool Equals(Array other)
        {
            return other != null
                && list.Count == other.list.Count
                && list.Zip(other.list, (left, right) => left?.Equals(right) ?? false).All(_ => _);
        }

        public override bool Equals(object other) => Equals(other as Array);

        public bool IsEmpty => Count == 0 && !list.Any();

        public string Join([Optional] string str = "")
        {
            if(str == null)
            {
                str = "";
            }
            return string.Join(str, list);
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

        public static string operator *(Array left, string right) => left.Join(right);

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