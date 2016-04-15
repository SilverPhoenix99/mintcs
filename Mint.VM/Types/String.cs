﻿using System;
using System.Text;

namespace Mint
{
    public class String : BaseObject
    {
        private StringBuilder value;

        public String() : base(Class.STRING)
        {
            Value = "";
        }

        public String(string value) : base(Class.STRING)
        {
            Value = value;
        }

        public String(String other) : this(other.Value) { }

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

        public static explicit operator String(string s) => new String(s);

        public static explicit operator string(String s) => s.Value;
    }
}
