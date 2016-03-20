using System;
using System.Reflection;

namespace Mint
{
    public class Hash : BaseObject
    {
        public Hash() : base(CLASS)
        {
            throw new NotImplementedException();
        }

        #region Static

        public static readonly Class CLASS;

        static Hash()
        {
            CLASS = new Class(new Symbol(MethodBase.GetCurrentMethod().DeclaringType.Name));
            //Object.DefineClass(CLASS);
        }

        #endregion
    }
}