using System.Dynamic;
using System.Linq.Expressions;

namespace Mint
{
    public struct Fixnum : iObject
    {
        public Fixnum(long value)
        {
            Value = value;
        }

        public long  Id                => (Value << 2) | 1;
        public Class Class             => Class.FIXNUM;
        public Class SingletonClass    { get { throw new TypeError("can't define singleton"); } }
        public Class CalculatedClass   => Class.FIXNUM;
        public bool  HasSingletonClass => false;
        public bool  Frozen            => true;
        public long  Value             { get; }

        public void Freeze() { }

        public override string ToString() => Value.ToString();

        public string Inspect() => ToString();

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public iObject Send(iObject name, params iObject[] args) => Object.Send(this, name, args);

        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);

        public static Fixnum operator -(Fixnum v) => new Fixnum(-v.Value);

        public static implicit operator Fixnum(long v) => new Fixnum(v);

        public static implicit operator long  (Fixnum s) => s.Value;

        public static explicit operator Fixnum(double v) => new Fixnum((long) v);

        public static explicit operator double(Fixnum v) => new Fixnum((long) v);

        public static explicit operator Fixnum(Float v)  => new Fixnum((long) v.Value);

        public static Fixnum operator +(Fixnum l, Fixnum r) => new Fixnum(l.Value + r.Value);
    }
}
