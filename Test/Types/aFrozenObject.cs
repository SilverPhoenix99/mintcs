using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Threading;

namespace Mint
{
    public abstract class aFrozenObject : iObject
    {
        private static long nextId = 0;

        public virtual  long  Id                { get; } = Interlocked.Increment(ref nextId) << 2;
        public abstract Class Class             { get; }
        public virtual  Class SingletonClass    { get { throw new TypeError("can't define singleton"); } }
        public virtual  Class CalculatedClass   => HasSingletonClass ? SingletonClass : Class;
        public virtual  bool  HasSingletonClass => false;
        public virtual  bool  Frozen            { get { return true; } protected set { /* noop */ } }

        public virtual void Freeze() { }

        public virtual string Inspect() => ToString();

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);
    }
}