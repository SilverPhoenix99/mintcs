namespace Mint.Compilation.Components
{
    internal class RightShiftCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.RSHIFT;

        public RightShiftCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
