namespace Mint.MethodBinding.Arguments
{
    public abstract partial class ArgumentKind
    {
        private class SimpleArgumentKind : ArgumentKind
        {
            public SimpleArgumentKind()
                : base("Simple")
            { }


            public override void Bundle(iObject argument, ArgumentBundle bundle)
            {
                bundle.Splat.Add(argument);
            }
        }
	}
}