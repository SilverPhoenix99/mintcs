using System.Dynamic;
using System.Linq.Expressions;

namespace Mint
{
    public struct FalseClass : iObject
    {
        public long  Id                => 0x0;
        public Class Class             => Class.FALSE;
        public Class SingletonClass    => Class.FALSE;
        public Class CalculatedClass   => Class.FALSE;
        public bool  HasSingletonClass => false;
        public bool  Frozen            => true;

        public void Freeze() { }

        public override string ToString() => "false";

        public string Inspect() => ToString();

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public iObject Send(iObject name, params iObject[] args) => Object.Send(this, name, args);

        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);

        public static implicit operator bool(FalseClass f) => false;
    }
}
