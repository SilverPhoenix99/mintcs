namespace Mint.Compilation.Components
{
    internal class UnaryPlusCompiler : UnaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.UPLUS;

        public UnaryPlusCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
