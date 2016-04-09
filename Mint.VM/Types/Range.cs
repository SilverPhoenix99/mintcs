using System.Reflection;

namespace Mint
{
    public class Range : BaseObject
    {
        public Range(iObject begin, iObject end, bool excludeEnd = false) : base(CLASS)
        {
            Begin = begin;
            End = end;
            ExcludeEnd = excludeEnd;
        }

        public iObject Begin      { get; }
        public iObject End        { get; }
        public bool    ExcludeEnd { get; }

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

        #region Static

        public static readonly Class CLASS;

        static Range()
        {
            CLASS = new Class(new Symbol(MethodBase.GetCurrentMethod().DeclaringType.Name));
            //DefineClass(CLASS);

            CLASS.DefineMethod("to_s", Reflector<Range>.Method(_ => _.ToString()));
            CLASS.DefineMethod("inspect", Reflector<Range>.Method(_ => _.Inspect()));
        }

        #endregion
    }
}
