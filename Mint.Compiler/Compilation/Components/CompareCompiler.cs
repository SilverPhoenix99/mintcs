namespace Mint.Compilation.Components
{
    internal class CompareCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.CMP;

        public CompareCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
