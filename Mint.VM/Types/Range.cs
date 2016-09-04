using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;

namespace Mint
{
    public class Range : BaseObject
    {
        public iObject Begin { get; }

        public iObject End { get; }

        public bool ExcludeEnd { get; }

        public Range(iObject begin, iObject end, bool excludeEnd = false) : base(Class.RANGE)
        {
            Debug.Assert(begin != null);
            Debug.Assert(end != null);

            Begin = begin;
            End = end;
            ExcludeEnd = excludeEnd;
        }

        public Range(Fixnum begin, Fixnum end, bool excludeEnd = false)
            : this((iObject) begin, end, excludeEnd)
        { }

        public bool Include(iObject value)
        {
            if(value is Fixnum)
            {
                return (Fixnum) Begin <= (Fixnum) value
                    && (ExcludeEnd ? (Fixnum) value < (Fixnum) End : (Fixnum) value <= (Fixnum) End);
            }

            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var s = ExcludeEnd ? "." : "";
            return $"{Begin}..{s}{End}";
        }

        public override string Inspect()
        {
            var s = ExcludeEnd ? "." : "";
            return $"{Begin.Inspect()}..{s}{End.Inspect()}";
        }

        public override bool Equals(object other) => Equals(other as Range);

        public bool Equals(Range other) =>
            ExcludeEnd.Equals(other?.ExcludeEnd)
            && Begin.Equals(other?.Begin)
            && End.Equals(other?.End);

        public override int GetHashCode()
        {
            var hash = 23;
            hash = hash * 31 + Begin.GetHashCode();
            return hash * 31 + End.GetHashCode();
        }

        public static class Reflection
        {
            public static readonly ConstructorInfo Ctor =
                Reflector.Ctor<Range>(typeof(iObject), typeof(iObject), typeof(bool));
        }

        public static class Expressions
        {
            public static NewExpression New(Expression begin, Expression end, Expression excludeEnd) =>
                Expression.New(Reflection.Ctor, begin, end, excludeEnd);
        }
    }
}
