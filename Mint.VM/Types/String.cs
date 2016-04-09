using System;
using System.Reflection;

namespace Mint
{
    public class String : BaseObject
    {
        private string value;

        public String() : base(CLASS)
        {
            Value = "";
        }

        public String(string value) : base(CLASS)
        {
            Value = value;
        }

        public String(String other) : this(other.Value) { }

        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                if(value == null) throw new ArgumentNullException(nameof(value));
                this.value = value;
            }
        }

        public String Concat(iObject other)
        {
            if(other is Fixnum )// TODO || other is Bignum)
            {
                // TODO: convert to codepoint
                throw new NotImplementedException();
            }

            if(!(other is String))
            {
                var type = (other == null || other is NilClass)
                         ? new String("nil")
                         : new String(other.Class.FullName);
                throw new TypeError($"no implicit conversion of {type} into String");
            }

            value += ((String) other).Value;
            return this;
        }

        // TODO: transform special chars into escapes
        public override string Inspect() => $"\"{Value}\"";

        public override string ToString() => Value;

        public static explicit operator String(string s) => new String(s);

        public static explicit operator string(String s) => s.Value;

        #region Static

        public static readonly Class CLASS;

        static String()
        {
            CLASS = new Class(new Symbol(MethodBase.GetCurrentMethod().DeclaringType.Name));
            //DefineClass(CLASS);

            CLASS.DefineMethod("to_s", Reflector<String>.Method(_ => _.ToString()));
            CLASS.DefineMethod("inspect", Reflector<String>.Method(_ => _.Inspect()));
        }

        #endregion
    }
}
