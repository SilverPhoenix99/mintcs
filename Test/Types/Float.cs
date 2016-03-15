using System;

namespace Mint
{
    public class Float : aObject
    {
        // TODO Superclass = Integer < Numeric
        public static Class CLASS = new Class(new Symbol("Float"));


        public Float(double value) : base()
        {
            Value = value;
        }


        public override Class  Class => CLASS;
        public          double Value { get; private set; }

        public override string ToString() => Value.ToString();

        public static explicit operator Float(double s) => new Float(s);

        public static explicit operator Float(float s) => new Float(s);

        public static explicit operator Float(Fixnum s) => new Float(s.Value);

        public static explicit operator double (Float s) => s.Value;


        static Float()
        {
            Object.DefineClass(CLASS);
        }
    }
}
