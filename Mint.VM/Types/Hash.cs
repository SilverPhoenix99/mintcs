using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mint
{
    public class Hash : BaseObject
    {
        private readonly LinkedDictionary<iObject, iObject> map;

        public Hash() : base(CLASS)
        {
            map = new LinkedDictionary<iObject, iObject>();
        }

        public Hash(int capacity) : base(CLASS)
        {
            map = new LinkedDictionary<iObject, iObject>(capacity);
        }

        public iObject this[iObject key]
        {
            get
            {
                iObject value;
                map.TryGetValue(key, out value);
                return value;
            }
            set { map[key] = value; }
        }

        public IEnumerable<iObject> Keys => map.Keys;

        public IEnumerable<iObject> Values => map.Values;

        public Array ToArray() => new Array(map.Select(_ => new Array(_.Key, _.Value)));

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