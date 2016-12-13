namespace Mint.Compilation.Components
{
    internal class BinaryOrCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.BIN_OR;

        public BinaryOrCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
