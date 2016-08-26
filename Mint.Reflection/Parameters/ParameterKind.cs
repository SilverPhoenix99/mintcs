namespace Mint.Reflection.Parameters
{
    public enum ParameterKind
    {
        //                          Min  Max
        Required,     // a           +1   +1
        Optional,     // a = nil     +0   +1
        Rest,         // *a          +0  Inf
        Block,        // &a          +0   +1
        KeyRequired,  // a:          +1   +1
        KeyOptional,  // a: nil      +0   +1
        KeyRest,      // **a         +0  Inf
        Parallel,     // (a, b)      +1   +1
    }
}
