using System;
using System.Reflection;

namespace Mint
{
    public class Range : BaseObject
    {
        public Range()
        {
            throw new NotImplementedException();
        }
        
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
