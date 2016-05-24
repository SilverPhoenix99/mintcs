namespace Mint.Binding.Arguments
{
    public abstract class ArgumentKind
    {
        public static ArgumentKind Simple = new SimpleArgumentKind();   // expr (any expression except others)
        public static ArgumentKind Rest = new RestArgumentKind();       // *expr
        public static ArgumentKind Key = new KeyArgumentKind();         // label: expr
        public static ArgumentKind KeyRest = new KeyRestArgumentKind(); // **expr
        public static ArgumentKind Block = new BlockArgumentKind();     // &expr or f(...) do |...| expr end

        public string Description { get; }

        protected ArgumentKind(string description)
        {
            Description = description;
        }

        public abstract void Bundle(iObject argument, ArgumentBundle bundle);

        public override string ToString() => Description;

        private class SimpleArgumentKind : ArgumentKind
        {
            public SimpleArgumentKind() : base("Simple")
            { }

            public override void Bundle(iObject argument, ArgumentBundle bundle)
            {
                bundle.Splat.Add(argument);
            }
        }

        private class RestArgumentKind : ArgumentKind
        {
            public RestArgumentKind() : base("Rest")
            { }

            public override void Bundle(iObject argument, ArgumentBundle bundle)
            {
                foreach(var item in (Array) argument)
                {
                    bundle.Splat.Add(item);
                }
            }
        }

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

        private class KeyRestArgumentKind : ArgumentKind
        {
            public KeyRestArgumentKind() : base("KeyRest")
            { }

            public override void Bundle(iObject argument, ArgumentBundle bundle)
            {
                foreach(var pair in (Hash) argument)
                {
                    var array = (Array) pair;
                    bundle.Keys[array[0]] = array[1];
                }
            }
        }

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