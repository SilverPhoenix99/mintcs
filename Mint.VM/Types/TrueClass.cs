using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mint
{
    public struct TrueClass : iObject
    {
        public long Id => 0x2;
        public Class Class => Class.TRUE;
        public Class SingletonClass => Class.TRUE;
        public Class EffectiveClass => Class.TRUE;
        public bool HasSingletonClass => false;
        public IEnumerable<Symbol> InstanceVariables => System.Array.Empty<Symbol>();
        public bool Frozen => true;


        public iObject Freeze()
            => this;


        public override string ToString()
            => "true";


        public string Inspect()
            => ToString();
        

        public override bool Equals(object other)
            => other is TrueClass || other as bool? == true;


        public override int GetHashCode()
            => Id.GetHashCode();


        public iObject InstanceVariableGet(Symbol name)
        {
            Object.ValidateInstanceVariableName(name.Name);
            return null;
        }


        public iObject InstanceVariableGet(string name)
            => InstanceVariableGet(new Symbol(name));


        public iObject InstanceVariableSet(Symbol name, iObject obj)
        {
            Object.ValidateInstanceVariableName(name.Name);
            throw new RuntimeError($"can't modify frozen {EffectiveClass.Name}");
        }


        public iObject InstanceVariableSet(string name, iObject obj)
            => InstanceVariableSet(new Symbol(name), obj);


        public static implicit operator bool(TrueClass t)
            => true;


        public static bool operator ==(TrueClass self, object other)
            => self.Equals(other);


        public static bool operator !=(TrueClass self, object other)
            => !self.Equals(other);


        public static class Expressions
        {
            public static readonly Expression Instance;


            static Expressions()
            {
                Instance = Expression.Constant(new TrueClass(), typeof(iObject));
            }
        }
    }
}
