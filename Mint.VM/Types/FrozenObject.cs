using System;
using System.Collections.Generic;
using System.Threading;
using Mint.MethodBinding;

namespace Mint
{
    public abstract class FrozenObject : iObject
    {
        private static long nextId = 4;

        [RubyMethod("object_id")]
        public virtual long Id { get; } = NextId();

        [RubyMethod("class")]
        public abstract Class Class { get; }

        [RubyMethod("singleton_class")]
        public virtual Class SingletonClass => throw new TypeError("can't define singleton");

        public virtual Class EffectiveClass => Class;

        public virtual bool HasSingletonClass => false;

        [RubyMethod("instance_variables")]
        public virtual IEnumerable<Symbol> InstanceVariables => System.Array.Empty<Symbol>();

        [RubyMethod("frozen?")]
        public virtual bool Frozen => true;

        [RubyMethod("freeze")]
        public virtual iObject Freeze() => this;
        
        [RubyMethod("inspect")]
        public virtual string Inspect() => ToString();
        
        [RubyMethod("to_s")]
        public override string ToString() => $"#<{Class.Name}:0x{Id:x}>";
        
        [RubyMethod("==")]
        [RubyMethod("===")]
        public override bool Equals(object other) => ReferenceEquals(this, other);
        
        [RubyMethod("hash")]
        public override int GetHashCode() => Id.GetHashCode();
        
        [RubyMethod("instance_variable_get")]
        public virtual iObject InstanceVariableGet(Symbol name)
        {
            Object.ValidateInstanceVariableName(name.Name);
            return null;
        }
        
        [RubyMethod("instance_variable_get")]
        public iObject InstanceVariableGet(string name) => InstanceVariableGet(new Symbol(name));
        
        [RubyMethod("instance_variable_set")]
        public virtual iObject InstanceVariableSet(Symbol name, iObject obj)
        {
            Object.ValidateInstanceVariableName(name.Name);
            throw new RuntimeError($"can't modify frozen {EffectiveClass.Name}");
        }
        
        [RubyMethod("instance_variable_set")]
        public iObject InstanceVariableSet(string name, iObject obj) => InstanceVariableSet(new Symbol(name), obj);
        
        internal static long NextId() => Interlocked.Add(ref nextId, 4);
    }
}