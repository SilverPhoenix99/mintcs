namespace Mint.MethodBinding.Arguments
{
    public abstract partial class ArgumentKind
    {
        public static readonly ArgumentKind Simple = new SimpleArgumentKind();   // expr (any expression except others)
        public static readonly ArgumentKind Rest = new RestArgumentKind();       // *expr
        public static readonly ArgumentKind Key = new KeyArgumentKind();         // label: expr
        public static readonly ArgumentKind KeyRest = new KeyRestArgumentKind(); // **expr
        public static readonly ArgumentKind Block = new BlockArgumentKind();     // &expr or f(...) do |...| expr end


        private ArgumentKind(string description)
        {
            Description = description;
        }


        public string Description { get; }


        public abstract void Bundle(iObject argument, ArgumentBundle bundle);


        public override string ToString()
            => Description;
    }
}