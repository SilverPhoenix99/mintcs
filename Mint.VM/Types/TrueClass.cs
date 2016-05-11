namespace Mint
{
    public struct TrueClass : iObject
    {
        public long  Id                => 0x2;
        public Class Class             => Class.TRUE;
        public Class SingletonClass    => Class.TRUE;
        public Class CalculatedClass   => Class.TRUE;
        public bool  HasSingletonClass => false;
        public bool  Frozen            => true;

        public void Freeze() { }

        public override string ToString() => "true";

        public string Inspect() => ToString();

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public iObject Send(iObject name, params iObject[] args) => Object.Send(this, name, args);

        public override bool Equals(object other) => other is TrueClass;

        public bool Equal(object other) => Equals(other);

        public override int GetHashCode() => Id.GetHashCode();
        
        public static implicit operator bool(TrueClass t) => true;

        public static bool operator ==(TrueClass self, object other) => self.Equals(other);

        public static bool operator !=(TrueClass self, object other) => !self.Equals(other);
    }
}
