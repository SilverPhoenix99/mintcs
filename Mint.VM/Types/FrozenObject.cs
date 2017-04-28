using System.Collections.Generic;
using System.Threading;

namespace Mint
{
    public abstract class FrozenObject : iObject
    {
        private static long nextId = 4;

        public virtual long Id { get; } = Interlocked.Add(ref nextId, 4);

        public abstract Class Class { get; }

        public virtual Class SingletonClass { get { throw new TypeError("can't define singleton"); } }

        public virtual Class EffectiveClass => Class;

        public virtual bool HasSingletonClass => false;

        public virtual IEnumerable<Symbol> InstanceVariables => System.Array.Empty<Symbol>();

        public virtual bool Frozen { get { return true; } protected set { /* noop */ } }

        public virtual iObject Freeze() => this;

        public virtual string Inspect() => ToString();

        public iObject Send(iObject name, params iObject[] args) => Object.Send(this, name, args);

        public override string ToString() => $"#<{Class.Name}:0x{Id:x}>";

        public override bool Equals(object other) => ReferenceEquals(this, other);

        public override int GetHashCode() => Id.GetHashCode();

        public virtual iObject InstanceVariableGet(Symbol name)
        {
            Object.ValidateInstanceVariableName(name.Name);
            return null;
        }

        public iObject InstanceVariableGet(string name) => InstanceVariableGet(new Symbol(name));

        public virtual iObject InstanceVariableSet(Symbol name, iObject obj)
        {
            Object.ValidateInstanceVariableName(name.Name);
            throw new RuntimeError($"can't modify frozen {EffectiveClass.Name}");
        }

        public iObject InstanceVariableSet(string name, iObject obj) => InstanceVariableSet(new Symbol(name), obj);
    }
}