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
        public double Value { get; }


        public override string ToString()
            => Value.ToString("0.0###############", CultureInfo.InvariantCulture);


        public override bool Equals(object other)
        {
            if(other is Float) return Equals((Float) other);
            if(other is Fixnum) return Value.Equals((double) (Fixnum) other);
            if(other is Bignum) return other.Equals(this);
            var instance = other as iObject;
            return instance != null && Object.ToBool(Class.EqOp.Call(instance, this));
        }


        public bool Equals(Float other)
            => Value.Equals(other?.Value);


        public override int GetHashCode()
            => Value.GetHashCode();


        public static Float operator -(Float v)
            => new Float(-v.Value);


        public static implicit operator Float(double v)
            => new Float(v);


        public static implicit operator double(Float v)
            => v.Value;


        public static explicit operator Float(float v)
            => new Float(v);


        public static explicit operator float (Float v)
            => (float) v.Value;


        public static explicit operator Float(Fixnum v)
            => new Float(v.Value);


        public static Float operator +(Float l, Float r)
            => new Float(l.Value + r.Value);


        public static Float operator +(Float l, Fixnum r)
            => new Float(l.Value + r.Value);


        public static Float operator -(Float l, Float r)
            => new Float(l.Value - r.Value);


        public static Float operator -(Float l, Fixnum r)
            => new Float(l.Value - r.Value);


        public static Float operator *(Float l, Float r)
            => new Float(l.Value * r.Value);


        public static Float operator *(Float l, Fixnum r)
            => new Float(l.Value * r.Value);


        public static Float operator /(Float l, Float r)
            => new Float(l.Value / r.Value);


        public static Float operator /(Float l, Fixnum r)
            => new Float(l.Value / r.Value);
    }
}
