namespace Mint.MethodBinding
{
    public enum Visibility
    {
        Public,    // anything.f()
        Protected, // self.f()
        Private,   // f()
    }
}
