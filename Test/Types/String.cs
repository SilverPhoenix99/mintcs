using System;

namespace Mint.Types
{
    public class String : Object
    {
        public static new readonly Class CLASS = new Class("String");


        private string value;

        
        public String() : base(CLASS)
        {
            Value = "";
        }
        

        public String(string value) : base(CLASS)
        {
            Value = value;
        }


        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                if(value == null) throw new ArgumentNullException("Value");
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
                var type = (other == null || other is Nil)
                         ? "nil"
                         : other.Class.Name;
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


        static String()
        {
            DefineClass(CLASS);
        }
    }
}
