using System;
using System.Reflection;

namespace Mint
{
    public class Regexp : aObject
    {
        public Regexp() : base(CLASS)
        {
            throw new NotImplementedException();
        }

        #region Static

        public static readonly Class CLASS;

        static Regexp()
        {
            CLASS = new Class(new Symbol(MethodBase.GetCurrentMethod().DeclaringType.Name));
            //DefineClass(CLASS);
        }

        #endregion
    }
}
