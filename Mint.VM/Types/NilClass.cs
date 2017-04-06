using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;

namespace Mint
{
    public struct NilClass : iObject
    {
        public long Id => 0x4;

        public Class Class => Class.NIL;

        public Class SingletonClass => Class.NIL;

        public Class EffectiveClass => Class.NIL;

        public bool HasSingletonClass => false;

        public IEnumerable<Symbol> InstanceVariables => System.Array.Empty<Symbol>();

        public bool Frozen => true;

        public iObject Freeze() => this;

        public override string ToString() => "";

        public string Inspect() => "nil";

        public iObject Send(iObject name, params iObject[] args) => Object.Send(this, name, args);

        public override bool Equals(object other) => IsNil(other);

        public override int GetHashCode() => Id.GetHashCode();

        public iObject InstanceVariableGet(Symbol name)
        {
            Object.ValidateInstanceVariableName(name.Name);
            return null;
        }

        public iObject InstanceVariableGet(string name) => InstanceVariableGet(new Symbol(name));

        public iObject InstanceVariableSet(Symbol name, iObject obj)
        {
            Object.ValidateInstanceVariableName(name.Name);
            throw new RuntimeError($"can't modify frozen {EffectiveClass.Name}");
        }

        public iObject InstanceVariableSet(string name, iObject obj) => InstanceVariableSet(new Symbol(name), obj);

        public static implicit operator bool (NilClass self) => false;

        public static bool operator ==(NilClass self, object other) => self.Equals(other);

        public static bool operator !=(NilClass self, object other) => !self.Equals(other);

        public static bool IsNil(object other) => other == null || other is NilClass;

        public static class Reflection
        {
            public static readonly MethodInfo IsNil = Reflector.Method(() => IsNil(default(object)));
        }

        public static class Expressions
        {
            public static readonly Expression Instance;

            static Expressions()
            {
                Instance = Expression.Constant(new NilClass(), typeof(iObject));
            }

            public static MethodCallExpression IsNil(Expression value) =>
                Expression.Call(Reflection.IsNil, value);
        }
    }
}
