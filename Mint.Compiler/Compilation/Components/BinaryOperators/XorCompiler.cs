namespace Mint.Compilation.Components
{
    internal class XorCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.XOR;

        public XorCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
