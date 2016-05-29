namespace Mint.Compilation.Components
{
    internal class BinaryAndCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.BIN_AND;

        public BinaryAndCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
