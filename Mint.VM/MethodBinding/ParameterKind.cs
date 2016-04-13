namespace Mint.MethodBinding
{
    public enum ParameterKind
    {
        Req,     // a
        Opt,     // a = nil
        Rest,    // *a
        Block,   // &a
        KeyReq,  // a:
        KeyOpt,  // a: nil
        KeyRest  // **a
    }
}