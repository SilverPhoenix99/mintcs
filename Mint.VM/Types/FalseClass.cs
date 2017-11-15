using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mint
{
    [RubyClass]
    public struct FalseClass : iObject
    {
        [RubyMethod("object_id")]
        public long Id => 0x0;

        [RubyMethod("class")]
        public Class Class => Class.FALSE;

        [RubyMethod("singleton_class")]
        public Class SingletonClass => Class.FALSE;

        public Class EffectiveClass => Class.FALSE;

        public bool HasSingletonClass => false;

        [RubyMethod("frozen?")]
        [RubyMethod("nil?")]
        public bool Frozen => true;

        [RubyMethod("instance_variables")]
        public IEnumerable<Symbol> InstanceVariables => System.Array.Empty<Symbol>();

        [RubyMethod("freeze")]
        public iObject Freeze() => this;

        [RubyMethod("to_s")]
        [RubyMethod("inspect")]
        public override string ToString() => "false";

        public string Inspect() => ToString();

        [RubyMethod("==")]
        [RubyMethod("===")]
        public override bool Equals(object other) => other is FalseClass || other as bool? == false;

        [RubyMethod("hash")]
        public override int GetHashCode() => Id.GetHashCode();

        [RubyMethod("instance_variable_get")]
        public iObject InstanceVariableGet(Symbol name)
        {
            Object.ValidateInstanceVariableName(name.Name);
            return null;
        }

        [RubyMethod("instance_variable_get")]
        public iObject InstanceVariableGet(string name) => InstanceVariableGet(new Symbol(name));

        [RubyMethod("instance_variable_set")]
        public iObject InstanceVariableSet(Symbol name, iObject obj)
        {
            Object.ValidateInstanceVariableName(name.Name);
            throw new RuntimeError($"can't modify frozen {EffectiveClass.Name}");
        }

        [RubyMethod("instance_variable_set")]
        public iObject InstanceVariableSet(string name, iObject obj) => InstanceVariableSet(new Symbol(name), obj);

        public static implicit operator bool(FalseClass f) => false;
        
        public static bool operator ==(FalseClass self, object other) => self.Equals(other);

        public static bool operator !=(FalseClass self, object other) => !self.Equals(other);

        public static class Expressions
        {
            public static readonly Expression Instance;
            
            static Expressions()
            {
                Instance = Expression.Constant(new FalseClass(), typeof(iObject));
            }
        }
    }
}
