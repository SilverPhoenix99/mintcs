namespace Mint.MethodBinding.Methods
{
    public class LocalVariable
    {
        public Symbol Name { get; }

        public iObject Value { get; set; }

        public LocalVariable(Symbol name, iObject value = null)
        {
            Name = name;
            Value = value;
        }
    }
}
