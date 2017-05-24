namespace Mint.MethodBinding.Arguments
{
    public abstract partial class ArgumentKind
    {
        private class KeyRestArgumentKind : ArgumentKind
        {
            public KeyRestArgumentKind()
                : base("KeyRest")
            { }


            public override void Bundle(iObject argument, ArgumentBundle bundle)
            {
                foreach(var pair in (Hash) argument)
                {
                    var array = (Array) pair;
                    bundle.Keywords[array[0]] = array[1];
                }
            }
        }
	}
}