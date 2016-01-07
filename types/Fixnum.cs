using System.Dynamic;
using System.Linq.Expressions;

namespace mint.types
{
    struct Fixnum : iObject
    {
        // TODO Superclass = Integer < Numeric
        public static Class CLASS = new Class(name: "Fixnum");

        private long value;

        public Fixnum(long value)
        {
            this.value = value << 2 | 1;
        }
        
        public long  Id                => value;
        public Class Class             => CLASS;
        public Class SingletonClass    { get { throw new TypeError("can't define singleton"); } }
        public Class RealClass         => CLASS;
        public bool  HasSingletonClass => false;
        public bool  Frozen            => true;
        public long Value              => value >> 2;

        public void Freeze() {}

        public override string ToString() => Value.ToString();
        
        public string Inspect() => ToString();
        
        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);
        
        public static implicit operator Fixnum(long v) => new Fixnum(v);

        public static explicit operator Fixnum(Float v) => new Fixnum((long) v.Value);
        
        public static explicit operator Fixnum(double v) => new Fixnum((long) v);
        
        public static implicit operator long (Fixnum s) => s.Value;

        static Fixnum()
        {
            Object.CLASS.Constants[CLASS.Name] = CLASS;
        }
    }
}
