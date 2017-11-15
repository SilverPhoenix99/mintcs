using System;
using System.Collections.Generic;

namespace Mint
{
    [RubyClass]
    public struct Fixnum : iObject
    {
        public const int BYTE_SIZE = sizeof(long);
        public const int BIT_SIZE = 8 * BYTE_SIZE;
        private const string RADIX = "0123456789abcdefghijklmnopqrstuvwxyz";

        public long Value { get; }

        public Fixnum(long value)
        {
            Value = value;
        }
        
        [RubyMethod("object_id")]
        public long Id => (Value << 1) | 1;

        [RubyMethod("class")]
        public Class Class => Class.FIXNUM;

        [RubyMethod("singleton_class")]
        public Class SingletonClass => throw new TypeError("can't define singleton");

        public Class EffectiveClass => Class.FIXNUM;

        public bool HasSingletonClass => false;

        [RubyMethod("instance_variables")]
        public IEnumerable<Symbol> InstanceVariables => System.Array.Empty<Symbol>();

        [RubyMethod("frozen?")]
        public bool Frozen => true;

        [RubyMethod("even?")]
        public bool IsEven => (Value & 1L) == 0L;

        [RubyMethod("odd?")]
        public bool IsOdd => (Value & 1L) == 1L;

        [RubyMethod("zero?")]
        public bool IsZero => Value == 0L;

        [RubyMethod("size")]
        public Fixnum Size => BYTE_SIZE;

        [RubyMethod("freeze")]
        public iObject Freeze() => this;

        [RubyMethod("to_s")]
        [RubyMethod("inspect")]
        public override string ToString() => ToString(10);

        [RubyMethod("to_s")]
        [RubyMethod("inspect")]
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

        public string Inspect() => ToString();
        
        [RubyMethod("hash")]
        public override int GetHashCode() => Value.GetHashCode();
        
        [RubyMethod("==")]
        [RubyMethod("===")]
        [RubyMethod("equal?")]
        public override bool Equals(object other)
        {
            switch(other)
            {
                case Fixnum _:
                    return Equals((Fixnum) other);
                case Bignum _:
                case Float _:
                    return other.Equals(this);
                case iObject vmObj:
                    return Object.ToBool(Class.EqOp.Call(vmObj, this));
                default:
                    return false;
            }
        }

        [RubyMethod("==")]
        [RubyMethod("===")]
        [RubyMethod("equal?")]
        public bool Equals(Fixnum other) => other.Value == Value;

        [RubyMethod("bit_length")]
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

        [RubyMethod("instance_variable_get")]
        public iObject InstanceVariableGet(Symbol name)
        {
            Object.ValidateInstanceVariableName(name.Name);
            return null;
        }

        [RubyMethod("instance_variable_get")]
        public iObject InstanceVariableGet(string name) => InstanceVariableGet(new Symbol(name));

        [RubyMethod("instance_variable_set")]
        public iObject InstanceVariableSet(Symbol name, iObject obj)
        {
            Object.ValidateInstanceVariableName(name.Name);
            throw new RuntimeError($"can't modify frozen {EffectiveClass.Name}");
        }

        [RubyMethod("instance_variable_set")]
        public iObject InstanceVariableSet(string name, iObject obj) => InstanceVariableSet(new Symbol(name), obj);

        // TODO: call Math.Abs directly instead of wrapping it.
        [RubyMethod("abs")]
        [RubyMethod("magnitude")]
        public Fixnum Abs() => Math.Abs(Value);

        [RubyMethod("-@")]
        public static Fixnum operator -(Fixnum v) => new Fixnum(-v.Value);

        [RubyMethod("+@")]
        public static Fixnum operator +(Fixnum v) => v;

        [RubyMethod("+")]
        public static Fixnum operator +(Fixnum l, Fixnum r) => new Fixnum(l.Value + r.Value);

        [RubyMethod("+")]
        public static Float operator +(Fixnum l, Float r) => new Float(l.Value + r.Value);

        [RubyMethod("-")]
        public static Fixnum operator -(Fixnum l, Fixnum r) => new Fixnum(l.Value - r.Value);

        [RubyMethod("-")]
        public static Float operator -(Fixnum l, Float r) => new Float(l.Value - r.Value);

        [RubyMethod("*")]
        public static Fixnum operator *(Fixnum l, Fixnum r) => new Fixnum(l.Value * r.Value);

        [RubyMethod("*")]
        public static Float operator *(Fixnum l, Float r) => new Float(l.Value * r.Value);

        [RubyMethod("/")]
        public static Fixnum operator /(Fixnum l, Fixnum r) => new Fixnum(l.Value / r.Value);

        [RubyMethod("/")]
        public static Float operator /(Fixnum l, Float r) => new Float(l.Value / r.Value);
        
        public static implicit operator Fixnum(long v) => new Fixnum(v);

        public static implicit operator long (Fixnum s) => s.Value;

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

        [RubyMethod("to_f")]
        public static explicit operator Float(Fixnum v) => new Float(v.Value);

        internal static ModuleBuilder<Fixnum> Build() =>
            ModuleBuilder<Fixnum>.DescribeClass(Class.INTEGER)
                .AutoDefineMethods()
        ;
    }
}
