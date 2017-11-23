using System;
using Mint.Reflection.Parameters.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.MethodBinding;
using Mint.Reflection;

namespace Mint
{
    /*
        ::methods:
          []  try_convert
        #methods:
          initialize       bsearch        dig         hash      permutation           rotate    sum
          initialize_copy  bsearch_index  drop        include?  pop                   rotate!   take
          &                clear          drop_while  index     product               sample    take_while
          *                collect        each        insert    push                  select    to_a
          +                collect!       each_index  inspect   rassoc                select!   to_ary
          -                combination    empty?      join      reject                shift     to_h
          <<               compact        eql?        keep_if   reject!               shuffle   to_s
          <=>              compact!       fetch       last      repeated_combination  shuffle!  transpose
          ==               concat         fill        length    repeated_permutation  size      uniq
          []               count          find_index  map       replace               slice     uniq!
          []=              cycle          first       map!      reverse               slice!    unshift
          any?             delete         flatten     max       reverse!              sort      values_at
          assoc            delete_at      flatten!    min       reverse_each          sort!     zip
          at               delete_if      frozen?     pack      rindex                sort_by!  |
     */

    [RubyClass]
    public class Array : BaseObject, IEnumerable<iObject>
    {
        private static readonly Comparer COMPARER = new Comparer();

        [ThreadStatic]
        private static ISet<Tuple<Array, Array>> equalsRecursionSet;
        
        private readonly List<iObject> list;
        
        public Array(IEnumerable<iObject> objs)
            : base(Class.ARRAY)
        {
            list = objs == null ? new List<iObject>() : new List<iObject>(objs);
        }
        
        public Array()
            : this((IEnumerable<iObject>) null)
        { }
        
        public Array(params iObject[] objs)
            : this((IEnumerable<iObject>) objs)
        { }
        
        public Array(int count, iObject obj)
            : this(Enumerable.Repeat(obj, count))
        { }
        
        public Array(int count)
            : this(count, new NilClass())
        { }

        [RubyMethod("count")]
        [RubyMethod("length")]
        [RubyMethod("size")]
        public int Count => list.Count;

        [RubyMethod("[]")]
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

        [RubyMethod("<<")]
        public Array Add(iObject element)
        {
            list.Add(element);
            return this;
        }

        public IEnumerator<iObject> GetEnumerator() => list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

        [RubyMethod("to_s")]
        public override string ToString() => $"[{string.Join(", ", list.Select(_ => _.Inspect()))}]";

        [RubyMethod("&")]
        public Array AndAlso(Array other)
        {
            var result = new Array(list);
            result.list.RemoveAll(item => !other.list.Contains(item));
            result.UniqSelf();
            return result;
        }

        [RubyMethod("first")]
        public iObject First => this[0];

        [RubyMethod("last")]
        public iObject Last => this[-1];

        [RubyMethod("clear")]
        public Array Clear() 
        {
            list.Clear();
            return this;
        }

        [RubyMethod("compact!")]
        public Array CompactSelf()
        {
            list.RemoveAll(NilClass.IsNil);
            return this;
        }

        [RubyMethod("compact")]
        public Array Compact() => new Array(list).CompactSelf();

        [RubyMethod("join")]
        public String Join([Optional] String str = null)
        {
            if(str == null)
            {
                str = new String("");
            }
            return new String(string.Join(str.ToString(), list));
        }

        [RubyMethod("replace")]
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

        [RubyMethod("reverse!")]
        public Array ReverseSelf()
        {
            list.Reverse();
            return this;
        }

        [RubyMethod("reverse")]
        public Array Reverse() => new Array(list).ReverseSelf();

        [RubyMethod("uniq!")]
        public Array UniqSelf()
        {
            var buffer = list.Distinct().ToList();
            list.Clear();
            list.AddRange(buffer);
            return this;
        }

        [RubyMethod("uniq")]
        public Array Uniq() => new Array(list).UniqSelf();

        [RubyMethod("==")]
        public override bool Equals(object other) => other is iObject && Equals((iObject) other);

        [RubyMethod("==")]
        public bool Equals(iObject other)
        {
            return other is Array ary
                ? Equals(ary)
                : Object.RespondTo(other, Symbol.TO_ARY) && Object.ToBool(Class.EqOp.Call(other, this));
        }

        [RubyMethod("==")]
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

        [RubyMethod("initialize", Visibility = Visibility.Private)]
        [RubyMethod("initialize_copy", Visibility = Visibility.Private)]
        [RubyMethod("*")]
        [RubyMethod("+")]
        [RubyMethod("-")]
        [RubyMethod("<=>")]
        [RubyMethod("any?")]
        [RubyMethod("assoc")]
        [RubyMethod("at")]
        [RubyMethod("bsearch")]
        [RubyMethod("bsearch_index")]
        [RubyMethod("collect")]
        [RubyMethod("collect!")]
        [RubyMethod("combination")]
        [RubyMethod("concat")]
        [RubyMethod("cycle")]
        [RubyMethod("delete")]
        [RubyMethod("delete_at")]
        [RubyMethod("delete_if")]
        [RubyMethod("dig")]
        [RubyMethod("drop")]
        [RubyMethod("drop_while")]
        [RubyMethod("each")]
        [RubyMethod("each_index")]
        [RubyMethod("empty?")]
        [RubyMethod("eql?")]
        [RubyMethod("fetch")]
        [RubyMethod("fill")]
        [RubyMethod("find_index")]
        [RubyMethod("flatten")]
        [RubyMethod("flatten!")]
        [RubyMethod("frozen?")]
        [RubyMethod("hash")]
        [RubyMethod("include?")]
        [RubyMethod("index")]
        [RubyMethod("insert")]
        [RubyMethod("inspect")]
        [RubyMethod("keep_if")]
        [RubyMethod("map")]
        [RubyMethod("map!")]
        [RubyMethod("max")]
        [RubyMethod("min")]
        [RubyMethod("pack")]
        [RubyMethod("permutation")]
        [RubyMethod("pop")]
        [RubyMethod("product")]
        [RubyMethod("push")]
        [RubyMethod("rassoc")]
        [RubyMethod("reject")]
        [RubyMethod("reject!")]
        [RubyMethod("repeated_combination")]
        [RubyMethod("repeated_permutation")]
        [RubyMethod("reverse_each")]
        [RubyMethod("rindex")]
        [RubyMethod("rotate")]
        [RubyMethod("rotate!")]
        [RubyMethod("sample")]
        [RubyMethod("select")]
        [RubyMethod("select!")]
        [RubyMethod("shift")]
        [RubyMethod("shuffle")]
        [RubyMethod("shuffle!")]
        [RubyMethod("slice")]
        [RubyMethod("slice!")]
        [RubyMethod("sort")]
        [RubyMethod("sort!")]
        [RubyMethod("sort_by!")]
        [RubyMethod("sum")]
        [RubyMethod("take")]
        [RubyMethod("take_while")]
        [RubyMethod("to_a")]
        [RubyMethod("to_ary")]
        [RubyMethod("to_h")]
        [RubyMethod("transpose")]
        [RubyMethod("unshift")]
        [RubyMethod("values_at")]
        [RubyMethod("zip")]
        [RubyMethod("|")]
        public void NotImplemented([Rest] Array args, [Block] object block)
            => throw new NotImplementedException(
                $"{nameof(Array)}#{CallFrame.Current.CallSite.MethodName.Name}"
            );

        internal static ModuleBuilder<Array> Build() =>
            ModuleBuilder<Array>.DescribeClass()
                .GenerateAllocator()
                .AutoDefineMethods()
        ;

        private class Comparer : IEqualityComparer<Tuple<Array, Array>>
        {
            public bool Equals(Tuple<Array, Array> x, Tuple<Array, Array> y)
                => ReferenceEquals(x, y)
                    || (ReferenceEquals(x.Item1, y.Item1) && ReferenceEquals(x.Item2, y.Item2))
                    || (ReferenceEquals(x.Item1, y.Item2) && ReferenceEquals(x.Item2, y.Item1));
            
            public int GetHashCode(Tuple<Array, Array> obj) => obj.Item1.GetHashCode() ^ obj.Item2.GetHashCode();
        }

        public static class Reflection
        {
            public static readonly ConstructorInfo CtorDefault = Reflector.Ctor<Array>();

            public static readonly ConstructorInfo Ctor = Reflector<Array>.Ctor<IEnumerable<iObject>>();
        }

        public static class Expressions
        {
            public static NewExpression New(Expression values)
                => Expression.New(Reflection.Ctor, values);


            public static NewExpression New()
                => Expression.New(Reflection.CtorDefault);
        }
    }
}
