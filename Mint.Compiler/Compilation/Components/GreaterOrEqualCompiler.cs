namespace Mint.Compilation.Components
{
    class GreaterOrEqualCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.GEQ;

        public GreaterOrEqualCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
