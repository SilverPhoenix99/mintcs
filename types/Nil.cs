using System.Dynamic;
using System.Linq.Expressions;

namespace mint.types
{
    struct Nil : iObject
    {
        public static readonly Class CLASS = new Class(name: "NilClass");

        public long  Id                => 0x2;
        public Class Class             => CLASS;
        public Class SingletonClass    => CLASS;
        public bool  HasSingletonClass => false;
        public Class RealClass         => CLASS;
        public bool  Frozen            => true;

        public void Freeze() {}

        public override string ToString() => "";

        public string Inspect() => "nil";

        public static implicit operator bool (Nil n) => false;

        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);

        static Nil()
        {
            Object.CLASS.Constants[CLASS.Name] = CLASS;
        }
    }
}
