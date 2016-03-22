using System;
using System.Reflection;

namespace Mint
{
    public class Array : BaseObject
    {
        public Array() : base(CLASS)
        {
            throw new NotImplementedException();
        }

        #region Static

        public static readonly Class CLASS;

        static Array()
        {
            CLASS = new Class(new Symbol(MethodBase.GetCurrentMethod().DeclaringType.Name));
            //Object.DefineClass(CLASS);
        }

        #endregion
    }
}