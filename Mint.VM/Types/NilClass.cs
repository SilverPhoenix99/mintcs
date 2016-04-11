using System.Dynamic;
using System.Linq.Expressions;

namespace Mint
{
    public struct NilClass : iObject
    {
        public long  Id                => 0x4;
        public Class Class             => CLASS;
        public Class SingletonClass    => CLASS;
        public Class CalculatedClass   => CLASS;
        public bool  HasSingletonClass => false;
        public bool  Frozen            => true;

        public void Freeze() { }

        public override string ToString() => "";

        public string Inspect() => "nil";

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public iObject Send(iObject name, params iObject[] args) => Object.Send(this, name, args);

        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);

        public override bool Equals(object other) => IsNil(other);

        public override int GetHashCode() => typeof(NilClass).GetHashCode();

        public static implicit operator bool (NilClass n) => false;

        public static bool operator ==(NilClass n, object other) => IsNil(other);

        public static bool operator !=(NilClass n, object other) => !IsNil(other);

        #region Static

        public static readonly Class CLASS;

        public static bool IsNil(object other) => other == null || other is NilClass;

        static NilClass()
        {
            CLASS = ClassBuilder<NilClass>.Describe()
                .DefMethod("to_s",    _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;
        }

        #endregion
    }
}
