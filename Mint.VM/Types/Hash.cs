using System;
using System.Reflection;

namespace Mint
{
    public class Hash : BaseObject
    {
        private readonly LinkedDictionary<iObject, iObject> map;

        public Hash() : base(CLASS)
        {
            map = new LinkedDictionary<iObject, iObject>();
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