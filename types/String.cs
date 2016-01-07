using System;

namespace mint.types
{
    class String : Object
    {
        public static new readonly Class CLASS = new Class(name: "String");


        private string value;


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

        // TODO
        public override string Inspect() => $"\"{Value}\"";


        public override string ToString() => Value;


        public static explicit operator String(string s) => new String(s);


        public static implicit operator string(String s) => s.Value;


        static String()
        {
            Object.CLASS.Constants[CLASS.Name] = CLASS;
        }
    }
}
