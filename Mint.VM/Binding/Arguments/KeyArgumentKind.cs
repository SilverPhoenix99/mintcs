namespace Mint.Binding.Arguments
{
    public abstract partial class ArgumentKind
    {
        private class KeyArgumentKind : ArgumentKind
        {
            public KeyArgumentKind() : base("Key")
            { }

            public override void Bundle(iObject argument, ArgumentBundle bundle)
            {
                var labeledArg = (Array) argument;
                bundle.Keys[labeledArg[0]] = labeledArg[1];
            }
        }
	}
}