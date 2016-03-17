using System.Dynamic;
using System.Linq.Expressions;

namespace Mint
{
    public struct False : iObject
    {
        public static readonly Class CLASS;

        public long  Id                => 0x0;
        public Class Class             => CLASS;
        public Class SingletonClass    => CLASS;
        public bool  HasSingletonClass => false;
        public Class CalculatedClass   => CLASS;
        public bool  Frozen            => true;

        public void Freeze() {}
        
        public override string ToString() => "false";

        public string Inspect() => ToString();

        public static implicit operator bool(False f) => false;

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);

        static False()
        {
            CLASS = new Class(new Symbol("FalseClass"));
            //Object.DefineClass(CLASS);
        }
    }
}
