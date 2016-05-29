namespace Mint.Compilation.Components
{
    internal class SubtractionCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.MINUS;

        public SubtractionCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
