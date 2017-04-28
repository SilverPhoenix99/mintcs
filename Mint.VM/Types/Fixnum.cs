using System;
using System.Collections.Generic;

namespace Mint
{
    public struct Fixnum : iObject
    {
        public const int BYTE_SIZE = sizeof(long);

        public const int BIT_SIZE = 8 * BYTE_SIZE;

        private const string RADIX = "0123456789abcdefghijklmnopqrstuvwxyz";

        public long Id => (Value << 1) | 1;

        public Class Class => Class.FIXNUM;

        public Class SingletonClass { get { throw new TypeError("can't define singleton"); } }

        public Class EffectiveClass => Class.FIXNUM;

        public bool HasSingletonClass => false;

        public IEnumerable<Symbol> InstanceVariables => System.Array.Empty<Symbol>();

        public bool Frozen => true;

        public long Value { get; }

        public Fixnum(long value)
        {
            Value = value;
        }

        public iObject Freeze() => this;

        public override string ToString() => ToString(10);

        public string ToString(int radix)
        {
            if(radix < 2 || radix > 36)
            {
                throw new ArgumentError($"invalid radix {radix}");
            }

            if(Value == 0)
            {
                return "0";
            }

            if(radix == 10)
            {
                return Value.ToString();
            }

            var sign = Value < 0;
            var value = Math.Abs(Value);
            var chars = new List<char>();

            while(value != 0)
            {
                var pos = (int) (value % radix);
                chars.Add(RADIX[pos]);
                value /= radix;
            }

            if(sign)
            {
                chars.Add('-');
            }

            chars.Reverse();

            return new string(chars.ToArray());
        }

        public override int GetHashCode() => Value.GetHashCode();

        public string Inspect() => ToString();

        public string Inspect(int radix) => ToString(radix);

        public iObject Send(iObject name, params iObject[] args) => Object.Send(this, name, args);

        public override bool Equals(object other)
        {
            if(other is Fixnum) return Equals((Fixnum) other);
            if(other is Bignum || other is Float) return other.Equals(this);
            var instance = other as iObject;
            return instance != null && Object.ToBool(Class.EqOp.Call(instance, this));
        }

        public bool Equals(Fixnum other) => other.Value == Value;

        public bool ReferenceEquals(Fixnum other) => Equals(other);

        public Fixnum BitLength() => BIT_SIZE - LeadingZeros();

        private Fixnum LeadingZeros()
        {
            var x = Value;
            if(x < 0)
            {
                x = ~x;
            }
            long n = BIT_SIZE;

            long y;
            y = x >> 32; if(y != 0) { n -= 32; x = y; }
            y = x >> 16; if(y != 0) { n -= 16; x = y; }
            y = x >>  8; if(y != 0) { n -=  8; x = y; }
            y = x >>  4; if(y != 0) { n -=  4; x = y; }
            y = x >>  2; if(y != 0) { n -=  2; x = y; }
            y = x >>  1; if(y != 0) { return n - 2; }
            return n - x;
        }

        public iObject InstanceVariableGet(Symbol name)
        {
            Object.ValidateInstanceVariableName(name.Name);
            return null;
        }

        public iObject InstanceVariableGet(string name) => InstanceVariableGet(new Symbol(name));

        public iObject InstanceVariableSet(Symbol name, iObject obj)
        {
            Object.ValidateInstanceVariableName(name.Name);
            throw new RuntimeError($"can't modify frozen {EffectiveClass.Name}");
        }

        public iObject InstanceVariableSet(string name, iObject obj) => InstanceVariableSet(new Symbol(name), obj);

        public static Fixnum operator -(Fixnum v) => new Fixnum(-v.Value);

        public static implicit operator Fixnum(long v) => new Fixnum(v);

        public static implicit operator long  (Fixnum s) => s.Value;

        public static implicit operator Fixnum(int v) => new Fixnum(v);

        public static explicit operator int(Fixnum s) => (int) s.Value;

        public static implicit operator Fixnum(uint v) => new Fixnum(v);

        public static explicit operator uint(Fixnum s) => (uint) s.Value;

        public static implicit operator Fixnum(short v) => new Fixnum(v);

        public static explicit operator short(Fixnum s) => (short) s.Value;

        public static implicit operator Fixnum(ushort v) => new Fixnum(v);

        public static explicit operator ushort(Fixnum s) => (ushort) s.Value;

        public static implicit operator Fixnum(sbyte v) => new Fixnum(v);

        public static explicit operator sbyte(Fixnum s) => (sbyte) s.Value;

        public static implicit operator Fixnum(byte v) => new Fixnum(v);

        public static explicit operator byte(Fixnum s) => (byte) s.Value;

        public static explicit operator Fixnum(double v) => new Fixnum((long) v);

        public static explicit operator double(Fixnum v) => v.Value;

        public static explicit operator Fixnum(Float v)  => new Fixnum((long) v.Value);

        public static Fixnum operator +(Fixnum l, Fixnum r) => new Fixnum(l.Value + r.Value);

        public static Float operator +(Fixnum l, Float r) => new Float(l.Value + r.Value);

        public static Fixnum operator -(Fixnum l, Fixnum r) => new Fixnum(l.Value - r.Value);

        public static Float operator -(Fixnum l, Float r) => new Float(l.Value - r.Value);

        public static Fixnum operator *(Fixnum l, Fixnum r) => new Fixnum(l.Value * r.Value);

        public static Float operator *(Fixnum l, Float r) => new Float(l.Value * r.Value);

        public static Fixnum operator /(Fixnum l, Fixnum r) => new Fixnum(l.Value / r.Value);

        public static Float operator /(Fixnum l, Float r) => new Float(l.Value / r.Value);
    }
}
