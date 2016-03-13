using System;
using System.Dynamic;
using System.Linq.Expressions;

namespace Mint.Types
{
    public struct Fixnum : iObject
    {
        public static Class NUMERIC_CLASS = new Class("Numeric");

        public static Class INTEGER_CLASS = new Class(NUMERIC_CLASS, "Integer");

        public static Class CLASS = new Class(INTEGER_CLASS, "Fixnum");



        public Fixnum(long value)
        {
            Value = value;
        }
        
        public long  Id                => Value << 2 | 1;
        public Class Class             => CLASS;
        public Class SingletonClass    { get { throw new TypeError("can't define singleton"); } }
        public Class RealClass         => CLASS;
        public bool  HasSingletonClass => false;
        public bool  Frozen            => true;
        public long Value              { get; }

        public void Freeze() {}

        public override string ToString() => Value.ToString();
        
        public string Inspect() => ToString();
        
        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public static implicit operator Fixnum(long v) => new Fixnum(v);

        public static explicit operator Fixnum(Float v) => new Fixnum((long) v.Value);
        
        public static explicit operator Fixnum(double v) => new Fixnum((long) v);
        
        public static implicit operator long (Fixnum s) => s.Value;

        static Fixnum()
        {
            Object.DefineClass(CLASS);
            Object.DefineClass(INTEGER_CLASS);
            Object.DefineClass(NUMERIC_CLASS);
        }
    }
}
