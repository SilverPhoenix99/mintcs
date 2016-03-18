using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Mint
{
    public struct Fixnum : iObject
    {
        public Fixnum(long value)
        {
            Value = value;
        }
        
        public long  Id                => Value << 2 | 1;
        public Class Class             => CLASS;
        public Class SingletonClass    { get { throw new TypeError("can't define singleton"); } }
        public Class CalculatedClass   => CLASS;
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

        #region Static

        public static readonly Class NUMERIC_CLASS;
        public static readonly Class INTEGER_CLASS;
        public static readonly Class CLASS;

        static Fixnum()
        {
            NUMERIC_CLASS = new Class(new Symbol("Numeric"));
            INTEGER_CLASS = new Class(NUMERIC_CLASS, new Symbol("Integer"));
            CLASS = new Class(INTEGER_CLASS, new Symbol(MethodBase.GetCurrentMethod().DeclaringType.Name));
            
            //Object.DefineClass(CLASS);
            //Object.DefineClass(INTEGER_CLASS);
            //Object.DefineClass(NUMERIC_CLASS);
        }

        #endregion
    }
}
