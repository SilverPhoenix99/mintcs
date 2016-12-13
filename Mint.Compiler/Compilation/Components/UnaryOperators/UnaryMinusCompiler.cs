namespace Mint.Compilation.Components
{
    internal class UnaryMinusCompiler : UnaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.UMINUS;

        public UnaryMinusCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
