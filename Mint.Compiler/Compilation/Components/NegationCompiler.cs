namespace Mint.Compilation.Components
{
    internal class NegationCompiler : UnaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.NEG;

        public NegationCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
