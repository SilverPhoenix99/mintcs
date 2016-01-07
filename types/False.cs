using System.Dynamic;
using System.Linq.Expressions;

namespace mint.types
{
    struct False : iObject
    {
        public static readonly Class CLASS = new Class(name: "FalseClass");

        public long  Id                => 0x12;
        public Class Class             => CLASS;
        public Class SingletonClass    => CLASS;
        public bool  HasSingletonClass => false;
        public Class RealClass         => CLASS;
        public bool  Frozen            => true;

        public void Freeze() {}
        
        public override string ToString() => "false";

        public string Inspect() => ToString();

        public static implicit operator bool(False f) => false;

        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);

        static False()
        {
            Object.CLASS.Constants[CLASS.Name] = CLASS;
        }
    }
}
