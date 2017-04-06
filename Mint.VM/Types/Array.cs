using System;
using Mint.Reflection.Parameters.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;

namespace Mint
{
    public class Array : BaseObject, IEnumerable<iObject>
    {
        private readonly List<iObject> list;

        public Array(IEnumerable<iObject> objs) : base(Class.ARRAY)
        {
            list = objs == null ? new List<iObject>() : new List<iObject>(objs);
        }

        public Array() : this((IEnumerable<iObject>) null)
        { }

        public Array(params iObject[] objs) : this((IEnumerable<iObject>) objs)
        { }

        public Array(int count, iObject obj) : this()
        {
            for(var i = 0; i < count; i++)
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
            list.RemoveAll(NilClass.IsNil);
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

        public override bool Equals(object other) => other is iObject && Equals((iObject) other);

        public bool Equals(iObject other)
        {
            return other is Array ary
                ? Equals(ary)
                : Object.RespondTo(other, Symbol.TO_ARY) && Object.ToBool(Class.EqOp.Call(other, this));
        }

        private class Comparer : IEqualityComparer<Tuple<Array, Array>>
        {
            public bool Equals(Tuple<Array, Array> x, Tuple<Array, Array> y) =>
                ReferenceEquals(x, y)
                || (ReferenceEquals(x.Item1, y.Item1) && ReferenceEquals(x.Item2, y.Item2))
                || (ReferenceEquals(x.Item1, y.Item2) && ReferenceEquals(x.Item2, y.Item1));

            public int GetHashCode(Tuple<Array, Array> obj) => obj.Item1.GetHashCode() ^ obj.Item2.GetHashCode();
        }

        private static readonly Comparer COMPARER = new Comparer();

        [ThreadStatic]
        private static ISet<Tuple<Array, Array>> equalsRecursionSet;

        public bool Equals(Array other)
        {
            if(ReferenceEquals(this, other))
            {
                return true;
            }

            if(other == null || Count != other.Count)
            {
                return false;
            }

            var topLevel = equalsRecursionSet == null;

            if(topLevel)
            {
                equalsRecursionSet = new HashSet<Tuple<Array, Array>>(COMPARER);
            }

            try
            {
                var pair = new Tuple<Array, Array>(this, other);

                if(equalsRecursionSet.Contains(pair))
                {
                    return true;
                }

                equalsRecursionSet.Add(pair);

                var elements = list.Zip(other.list, (l, r) => Class.EqOp.Call(l, r));
                return elements.Select(Object.ToBool).All(eq => eq);
            }
            finally
            {
                if(topLevel)
                {
                    equalsRecursionSet = null;
                }
            }
        }

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
            result.list.RemoveAll(right.Contains);
            return result;
        }

        public static explicit operator Array(iObject[] objects) => new Array(objects);

        public static class Reflection
        {
            public static readonly ConstructorInfo CtorDefault = Reflector.Ctor<Array>();

            public static readonly ConstructorInfo Ctor = Reflector<Array>.Ctor<IEnumerable<iObject>>();
        }

        public static class Expressions
        {
            public static NewExpression New(Expression values) => Expression.New(Reflection.Ctor, values);

            public static NewExpression New() => Expression.New(Reflection.CtorDefault);
        }
    }
}
