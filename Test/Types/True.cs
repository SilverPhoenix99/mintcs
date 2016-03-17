using System.Dynamic;
using System.Linq.Expressions;

namespace Mint
{
    public struct True : iObject
    {
        public static readonly Class CLASS;

        public long  Id                => 0x2;
        public Class Class             => CLASS;
        public Class SingletonClass    => CLASS;
        public bool  HasSingletonClass => false;
        public Class CalculatedClass   => CLASS;
        public bool  Frozen            => true;

        public void Freeze() {}
        
        public override string ToString() => "true";

        public string Inspect() => ToString();

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public static implicit operator bool(True t) => true;

        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);

        static True()
        {
            CLASS = new Class(new Symbol("TrueClass"));
            //Object.DefineClass(CLASS);
        }
    }
}
