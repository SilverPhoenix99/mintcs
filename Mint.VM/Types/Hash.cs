using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mint
{
    public class Hash : BaseObject, IEnumerable<iObject>
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

        public IEnumerator<iObject> GetEnumerator()
        {
            foreach(var element in map.Select(_ => (iObject) new Array(_.Key, _.Value)))
            {
                yield return element;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString()
        {
            var elements = map.Select(_ => $"{_.Key.Inspect()}=>{_.Value.Inspect()}");
            return $"{{{string.Join(", ", elements)}}}";
        }

        #region Static

        public static readonly Class CLASS;
        internal static readonly Symbol TO_HASH;

        static Hash()
        {
            CLASS = new Class(new Symbol(MethodBase.GetCurrentMethod().DeclaringType.Name));
            TO_HASH = new Symbol("to_hash");
            //Object.DefineClass(CLASS);

            CLASS.DefineMethod("to_s", Reflector<Hash>.Method(_ => _.ToString()));
            CLASS.DefineMethod("inspect", Reflector<Hash>.Method(_ => _.Inspect()));
        }

        // Double splat to hash: **other
        internal static iObject Cast(iObject other)
        {
            if(!other.IsA(CLASS))
            {
                if(!other.CalculatedClass.IsDefined(TO_HASH))
                {
                    throw new TypeError($"no implicit conversion of {other.Class.FullName} into Hash");
                }

                var result = other.Send(TO_HASH);
                if(!result.IsA(CLASS))
                {
                    throw new TypeError($"can't convert {other.Class.FullName} to Hash"
                        + $" ({other.Class.FullName}#to_hash gives {result.Class.FullName})");
                }

                other = result;
            }

            return other;
        }

        #endregion
    }
}