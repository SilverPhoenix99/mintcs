namespace Mint.Compilation.Components
{
    internal class MultiplationCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.MUL;

        public MultiplationCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
