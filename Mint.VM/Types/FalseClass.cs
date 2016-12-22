using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mint
{
    public struct FalseClass : iObject
    {
        public long Id => 0x0;

        public Class Class => Class.FALSE;

        public Class SingletonClass => Class.FALSE;

        public Class EffectiveClass => Class.FALSE;

        public bool HasSingletonClass => false;

        public bool Frozen => true;

        public IEnumerable<Symbol> InstanceVariables => System.Array.Empty<Symbol>();

        public iObject Freeze() => this;

        public override string ToString() => "false";

        public string Inspect() => ToString();

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public iObject Send(iObject name, params iObject[] args) => Object.Send(this, name, args);

        public override bool Equals(object other) => other is FalseClass || other as bool? == false;

        public bool Equal(object other) => Equals(other);

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
            throw new RuntimeError($"can't modify frozen {EffectiveClass.FullName}");
        }

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
