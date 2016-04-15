namespace Mint.MethodBinding
{
    public enum ParameterKind
    {
        Required,     // a
        Optional,     // a = nil
        Rest,         // *a
        Block,        // &a
        KeyRequired,  // a:
        KeyOptional,  // a: nil
        KeyRest,      // **a
        Parallel,     // (a, b)
    }
}