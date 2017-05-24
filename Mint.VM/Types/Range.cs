using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;

namespace Mint
{
    public class Range : BaseObject
    {
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


        public Range()
            : this(new NilClass(), new NilClass())
        { }


        public iObject Begin { get; }
        public iObject End { get; }
        public bool ExcludeEnd { get; }


        public bool Include(iObject value)
        {
            if(Begin is Fixnum fixnumBegin && End is Fixnum fixnumEnd)
            {
                return value is Fixnum fixnumValue
                    && fixnumBegin <= fixnumValue
                    && (ExcludeEnd ? fixnumValue < fixnumEnd : fixnumValue <= fixnumEnd);
            }

            if(Begin is String && End is String)
            {
                if(!(value is String))
                {
                    return false;
                }

                var text = value.ToString();
                return string.Compare(Begin.ToString(), text, StringComparison.Ordinal) <= 0
                    && (
                        ExcludeEnd ? string.Compare(End.ToString(), text, StringComparison.Ordinal) > 0
                        : string.Compare(End.ToString(), text, StringComparison.Ordinal) >= 0
                    );
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


        public override bool Equals(object other)
            => Equals(other as Range);


        public bool Equals(Range other)
            => ExcludeEnd.Equals(other?.ExcludeEnd)
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
            public static readonly ConstructorInfo Ctor = Reflector<Range>.Ctor<iObject, iObject, bool>();
        }


        public static class Expressions
        {
            public static NewExpression New(Expression begin, Expression end, Expression excludeEnd)
                => Expression.New(Reflection.Ctor, begin, end, excludeEnd);
        }
    }
}
