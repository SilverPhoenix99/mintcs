namespace Mint
{
    public struct FalseClass : iObject
    {
        public long  Id                => 0x0;
        public Class Class             => Class.FALSE;
        public Class SingletonClass    => Class.FALSE;
        public Class CalculatedClass   => Class.FALSE;
        public bool  HasSingletonClass => false;
        public bool  Frozen            => true;

        public void Freeze() { }

        public override string ToString() => "false";

        public string Inspect() => ToString();

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public iObject Send(iObject name, params iObject[] args) => Object.Send(this, name, args);

        public override bool Equals(object other) => other is FalseClass;

        public bool Equal(object other) => Equals(other);

        public override int GetHashCode() => Id.GetHashCode();
        
        public static implicit operator bool(FalseClass f) => false;

        public static bool operator ==(FalseClass self, object other) => self.Equals(other);

        public static bool operator !=(FalseClass self, object other) => !self.Equals(other);
    }
}
