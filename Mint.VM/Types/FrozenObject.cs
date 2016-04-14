using System.Dynamic;
using System.Linq.Expressions;
using System.Threading;

namespace Mint
{
    public abstract class FrozenObject : iObject
    {
        private static long nextId;

        public virtual  long  Id                { get; } = Interlocked.Increment(ref nextId) << 2;
        public abstract Class Class             { get; }
        public virtual  Class SingletonClass    { get { throw new TypeError("can't define singleton"); } }
        public virtual  Class CalculatedClass   => HasSingletonClass ? SingletonClass : Class;
        public virtual  bool  HasSingletonClass => false;
        public virtual  bool  Frozen            { get { return true; } protected set { /* noop */ } }

        public virtual void Freeze() { }

        public virtual string Inspect() => ToString();

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public iObject Send(iObject name, params iObject[] args) => Object.Send(this, name, args);

        //public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);

        public override string ToString() => $"#<{Class.FullName}:0x{Id:x}>";

        public virtual bool Equal(object other) => Equals(other);

        public override bool Equals(object other) => ReferenceEquals(this, other);

        public override int GetHashCode() => Id.GetHashCode();
    }
}