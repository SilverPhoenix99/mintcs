using System.Globalization;

namespace Mint
{
    public class Float : FrozenObject
    {
        public override Class  Class => Class.FLOAT;

        public double Value { get; }

        public Float(double value)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString("0.0###############", CultureInfo.InvariantCulture);

        public override bool Equal(object other) => (other as Float)?.Value == Value;

        public override bool Equals(object other)
        {
            if(other is Float)  return Equal(other);
            if(other is Fixnum) return (double) ((Fixnum) other).Value == Value;
            // TODO Complex and Rational
            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static Float operator -(Float v) => new Float(-v.Value);

        public static implicit operator Float(double v) => new Float(v);

        public static implicit operator double(Float v) => v.Value;

        public static explicit operator Float(float v) => new Float(v);

        public static explicit operator float (Float v) => (float) v.Value;

        public static explicit operator Float(Fixnum v) => new Float(v.Value);
    }
}
