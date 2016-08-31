namespace Mint.MethodBinding.Methods
{
    public class LocalVariable
    {
        public Symbol Name { get; }

        public iObject Value { get; }

        public LocalVariable(Symbol name, iObject value = null)
        {
            Name = name;
            Value = value ?? new NilClass();
        }
    }
}
