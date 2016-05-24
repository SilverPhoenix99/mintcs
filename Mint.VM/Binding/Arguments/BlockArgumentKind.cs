namespace Mint.Binding.Arguments
{
    public abstract partial class ArgumentKind
    {
        private class BlockArgumentKind : ArgumentKind
        {
            public BlockArgumentKind() : base("Block")
            { }

            public override void Bundle(iObject argument, ArgumentBundle bundle)
            {
                bundle.Block = argument;
            }
        }
    }
}