using System;
using System.Text;
using Mint.Reflection.Parameters.Attributes;

namespace Mint
{
    public class String : BaseObject
    {
        private StringBuilder value;

        public String(string value) : base(Class.STRING)
        {
            Value = value;
        }

        public String() : this("")
        { }

        public String(StringBuilder value) : this(value.ToString())
        { }

        public String(String other) : this(other.Value)
        { }

        public string Value
        {
            get
            {
                return value.ToString();
            }
            set
            {
                if(value == null) throw new ArgumentNullException(nameof(value));
                this.value = new StringBuilder(value);
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

            value.Append( ((String) other).Value );
            return this;
        }

        // TODO: transform special chars into escapes
        public override string Inspect() => $"\"{Value}\"";

        public override string ToString() => Value;

        public bool Equals(String other) => other != null && other.value.Equals(value);

        public override bool Equals(object other) => Equals(other as String);

        public override int GetHashCode() => Value.GetHashCode();

        public static explicit operator String(string s) => new String(s);

        public static explicit operator string(String s) => s.Value;
        
        public String LeftJustify(int length, [Optional] string padstr = " ")
        {
            if(padstr == null)
            {
                padstr = " ";
            }
        
            throw new System.NotImplementedException();
        }
    }
}
