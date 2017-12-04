using System;
using System.Collections.Generic;
using System.Globalization;

namespace Mint
{
    [RubyClass]
    public readonly struct Float : iObject
    {
        private static readonly IDictionary<double, long> ID_MAP = new Dictionary<double, long>();

        public double Value { get; }

        public Float(double value)
        {
            Value = value;
        }

        [RubyMethod("object_id")]
        public long Id
        {
            get
            {
                lock(ID_MAP)
                {
                    return ID_MAP.TryGetValue(Value, out var id) ? id : ID_MAP[Value] = FrozenObject.NextId();
                }
            }
        }

        [RubyMethod("class")]
        public Class Class => Class.FLOAT;

        [RubyMethod("singleton_class")]
        public Class SingletonClass => throw new TypeError("can't define singleton");

        public Class EffectiveClass => Class.FLOAT;

        public bool HasSingletonClass => false;

        [RubyMethod("instance_variables")]
        public IEnumerable<Symbol> InstanceVariables => System.Array.Empty<Symbol>();

        [RubyMethod("frozen?")]
        public bool Frozen => true;

        [RubyMethod("freeze")]
        public iObject Freeze() => this;

        [RubyMethod("to_s")]
        [RubyMethod("inspect")]
        public override string ToString() => Value.ToString("0.0###############", CultureInfo.InvariantCulture);

        public string Inspect() => ToString();

        [RubyMethod("hash")]
        public override int GetHashCode() => Value.GetHashCode();

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
                case iObject obj:
                    return Object.ToBool(Class.EqOp.Call(obj, this));
            }
            return false;
        }

        [RubyMethod("==")]
        [RubyMethod("===")]
        public bool Equals(Float other) => Value.Equals(other.Value);

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

        internal static ModuleBuilder<Float> Build() =>
            ModuleBuilder<Float>.DescribeClass(Class.NUMERIC)
                .AutoDefineMethods()
        ;
    }
}
