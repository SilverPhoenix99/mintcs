using System;
using System.Globalization;

namespace Mint
{
    [RubyClass]
    public class Float : FrozenObject
    {
        public double Value { get; }

        public Float(double value)
        {
            Value = value;
        }

        [RubyMethod("class")]
        public override Class Class => Class.FLOAT;

        [RubyMethod("to_s")]
        [RubyMethod("inspect")]
        public override string ToString() => Value.ToString("0.0###############", CultureInfo.InvariantCulture);

        [RubyMethod("==")]
        [RubyMethod("===")]
        public override bool Equals(object other)
        {
            switch(other)
            {
                case Float _:
                    return Equals((Float) other);
                case Fixnum _:
                    return Value.Equals((double) (Fixnum) other);
                case Bignum _:
                    return other.Equals(this);
                case iObject instance:
                    return Object.ToBool(Class.EqOp.Call(instance, this));
            }
            return false;
        }

        [RubyMethod("==")]
        [RubyMethod("===")]
        public bool Equals(Float other) => Value.Equals(other?.Value);

        [RubyMethod("hash")]
        public override int GetHashCode() => Value.GetHashCode();

        [RubyMethod("abs")]
        public Float Abs() => Math.Abs(Value);

        [RubyMethod("-@")]
        public static Float operator -(Float v) => new Float(-v.Value);

        [RubyMethod("+@")]
        public static Float operator +(Float v) => v;

        [RubyMethod("+")]
        public static Float operator +(Float l, Float r) => new Float(l.Value + r.Value);

        [RubyMethod("+")]
        public static Float operator +(Float l, Fixnum r) => new Float(l.Value + r.Value);

        [RubyMethod("-")]
        public static Float operator -(Float l, Float r) => new Float(l.Value - r.Value);

        [RubyMethod("-")]
        public static Float operator -(Float l, Fixnum r) => new Float(l.Value - r.Value);
        
        [RubyMethod("*")]
        public static Float operator *(Float l, Float r) => new Float(l.Value * r.Value);
        
        [RubyMethod("*")]
        public static Float operator *(Float l, Fixnum r) => new Float(l.Value * r.Value);
        
        [RubyMethod("/")]
        public static Float operator /(Float l, Float r) => new Float(l.Value / r.Value);
        
        [RubyMethod("/")]
        public static Float operator /(Float l, Fixnum r) => new Float(l.Value / r.Value);

        public static implicit operator Float(double v) => new Float(v);
        
        public static implicit operator double(Float v) => v.Value;
        
        public static explicit operator Float(float v) => new Float(v);
        
        public static explicit operator float (Float v) => (float) v.Value;

        [RubyMethod("to_i")]
        public static explicit operator Fixnum(Float v) => new Fixnum((long) v.Value);
    }
}
