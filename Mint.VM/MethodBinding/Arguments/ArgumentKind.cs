namespace Mint.MethodBinding.Arguments
{
    public abstract partial class ArgumentKind
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
    }
}