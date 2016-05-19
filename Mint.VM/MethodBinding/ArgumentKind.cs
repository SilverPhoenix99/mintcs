namespace Mint.MethodBinding
{
    public enum ArgumentKind
    {
        Simple,  // expr (any expression except others)
        Rest,    // *expr
        Key,     // label: expr
        KeyRest, // **expr
        Block    // &expr or f(...) do |...| expr end
    }
}