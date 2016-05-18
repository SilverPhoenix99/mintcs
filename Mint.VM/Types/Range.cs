using System;
using System.Diagnostics;

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

        public iObject Begin      { get; }
        public iObject End        { get; }
        public bool    ExcludeEnd { get; }

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
    }
}
