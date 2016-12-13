namespace Mint.Compilation.Components
{
    internal class LeftShiftCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.LSHIFT;

        public LeftShiftCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
