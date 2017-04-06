using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Mint.Reflection;
using Mint.Reflection.Parameters.Attributes;

namespace Mint
{
    public class String : BaseObject
    {
        private StringBuilder value;

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

        public String(string value) : base(Class.STRING)
        {
            Value = value;
        }

        public String() : this("")
        { }

        public String(object value) : this(value.ToString())
        { }

        public String Concat(iObject other)
        {
            if(other is Fixnum )// TODO || other is Bignum)
            {
                // TODO: convert to codepoint
                throw new NotImplementedException();
            }

            if(!(other is String))
            {
                var type = NilClass.IsNil(other) ? "nil" : other.Class.Name;
                throw new TypeError($"no implicit conversion of {type} into String");
            }

            value.Append( ((String) other).Value );
            return this;
        }

        public String Concat(string content)
        {
            content = content ?? "";
            value.Append(content);
            return  this;
        }

        // TODO: transform special chars into escapes
        public override string Inspect() => $"\"{Value}\"";

        public override string ToString() => Value;

        public bool Equals(String other) => other != null && other.value.Equals(value);

        public override bool Equals(object other) => Equals(other as String);

        public override int GetHashCode() => Value.GetHashCode();

        public static explicit operator String(string s) => new String(s);

        public static explicit operator string(String s) => s.Value;

        private const string DFT_PADSTR = " ";
        public String LeftJustify(int length, [Optional] string padstr = DFT_PADSTR)
        {
            if(padstr == null)
            {
                padstr = DFT_PADSTR;
            }

            length -= value.Length;
            if(length <= 0)
            {
                return new String(Value);
            }

            var result = new StringBuilder(Value);

            var count = length / padstr.Length;
            if(count > 0)
            {
                result.Append(string.Concat(Enumerable.Repeat(padstr, count)));
            }

            count = length % padstr.Length;
            if(count > 0)
            {
                result.Append(padstr, 0, count);
            }

            return new String(result.ToString());
        }

        public static class Reflection
        {
            public static readonly ConstructorInfo Ctor1 = Reflector.Ctor<String>();

            public static readonly ConstructorInfo Ctor2 = Reflector<String>.Ctor<string>();

            public static readonly ConstructorInfo Ctor3 = Reflector<String>.Ctor<object>();

            public static readonly MethodInfo Concat = Reflector<String>.Method(_ => _.Concat(default(string)));
        }

        public static class Expressions
        {
            public static NewExpression New() => Expression.New(Reflection.Ctor1);

            public static NewExpression New(Expression value)
            {
                value = value.StripConversions();

                if(value.Type == typeof(string))
                {
                    return Expression.New(Reflection.Ctor2, value);
                }

                value = value.Cast<object>();
                return Expression.New(Reflection.Ctor3, value);
            }

            public static MethodCallExpression Concat(Expression instance, Expression value) =>
                Expression.Call(instance, Reflection.Concat, value);
        }
    }
}
