using System.Globalization;

namespace Mint
{
    public class Float : FrozenObject
    {
        public Float(double value)
        {
            Value = value;
        }

        public override Class  Class => Class.FLOAT;
        public          double Value { get; }

        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

        public static Float operator -(Float v) => new Float(-v.Value);

        public static explicit operator Float(double v) => new Float(v);

        public static explicit operator double(Float v) => v.Value;

        public static explicit operator Float(float v) => new Float(v);

        public static explicit operator float (Float v) => (float) v.Value;

        public static explicit operator Float(Fixnum v) => new Float(v.Value);
    }
}
