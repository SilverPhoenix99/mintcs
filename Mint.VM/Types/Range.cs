using System.Reflection;

namespace Mint
{
    public class Range : BaseObject
    {
        public Range(iObject begin, iObject end, bool excludeEnd = false)
        {
            Begin = begin;
            End = end;
            ExcludeEnd = excludeEnd;
        }

        public iObject Begin      { get; }
        public iObject End        { get; }
        public bool    ExcludeEnd { get; }

        #region Static

        public static readonly Class CLASS;

        static Range()
        {
            CLASS = new Class(new Symbol(MethodBase.GetCurrentMethod().DeclaringType.Name));
            //DefineClass(CLASS);
        }

        #endregion
    }
}
