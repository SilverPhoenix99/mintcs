namespace Mint.MethodBinding.Arguments
{
    public abstract partial class ArgumentKind
    {
        private class RestArgumentKind : ArgumentKind
        {
            public RestArgumentKind()
                : base("Rest")
            { }


            public override void Bundle(iObject argument, ArgumentBundle bundle)
            {
                foreach(var item in (Array) argument)
                {
                    bundle.Splat.Add(item);
                }
            }
        }
	}
}