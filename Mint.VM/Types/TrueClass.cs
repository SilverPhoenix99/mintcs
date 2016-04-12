using System.Dynamic;
using System.Linq.Expressions;

namespace Mint
{
    public struct TrueClass : iObject
    {
        public long  Id                => 0x2;
        public Class Class             => Class.TRUE;
        public Class SingletonClass    => Class.TRUE;
        public Class CalculatedClass   => Class.TRUE;
        public bool  HasSingletonClass => false;
        public bool  Frozen            => true;

        public void Freeze() { }

        public override string ToString() => "true";

        public string Inspect() => ToString();

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public iObject Send(iObject name, params iObject[] args) => Object.Send(this, name, args);

        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);

        public static implicit operator bool(TrueClass t) => true;
    }
}
