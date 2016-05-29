namespace Mint.Compilation.Components
{
    internal class PowerCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.POW;

        public PowerCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
