namespace Mint.MethodBinding.Arguments
{
    public abstract partial class ArgumentKind
    {
        private class KeyArgumentKind : ArgumentKind
        {
            public KeyArgumentKind()
                : base("Key")
            { }


            public override void Bundle(iObject argument, ArgumentBundle bundle)
            {
                var labeledArg = (Array) argument;
                bundle.Keywords[labeledArg[0]] = labeledArg[1];
            }
        }
	}
}