using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;

namespace Mint
{
    [RubyClass]
    public struct NilClass : iObject
    {
        [RubyMethod("object_id")]
        public long Id => 0x4;

        [RubyMethod("class")]
        public Class Class => Class.NIL;

        [RubyMethod("singleton_class")]
        public Class SingletonClass => Class.NIL;

        public Class EffectiveClass => Class.NIL;

        public bool HasSingletonClass => false;

        [RubyMethod("instance_variables")]
        public IEnumerable<Symbol> InstanceVariables => System.Array.Empty<Symbol>();

        [RubyMethod("frozen?")]
        public bool Frozen => true;
        
        [RubyMethod("freeze")]
        public iObject Freeze() => this;
        
        [RubyMethod("to_s")]
        public override string ToString() => "";
        
        [RubyMethod("inspect")]
        public string Inspect() => "nil";
        
        [RubyMethod("==")]
        [RubyMethod("===")]
        public override bool Equals(object other) => IsNil(other);

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

        [RubyMethod("instance_variable_set")]
        public static implicit operator bool (NilClass self) => false;

        public static bool operator ==(NilClass self, object other) => self.Equals(other);

        public static bool operator !=(NilClass self, object other) => !self.Equals(other);

        [RubyMethod("to_a")]
        public static explicit operator Array(NilClass f) => new Array();

        [RubyMethod("to_f")]
        public static explicit operator Float(NilClass f) => new Float(0.0);

        [RubyMethod("to_i")]
        public static explicit operator Fixnum(NilClass f) => new Fixnum();

        [RubyMethod("to_h")]
        public static explicit operator Hash(NilClass f) => new Hash();

        [RubyMethod("nil?")]
        public static bool IsNil(object other) => other == null || other is NilClass;
        
        public static class Reflection
        {
            public static readonly MethodInfo IsNil = Reflector.Method(() => IsNil(default));
        }

        public static class Expressions
        {
            public static readonly Expression Instance;
            
            static Expressions()
            {
                Instance = Expression.Constant(new NilClass(), typeof(iObject));
            }
            
            public static MethodCallExpression IsNil(Expression value) => Expression.Call(Reflection.IsNil, value);
        }
    }
}
