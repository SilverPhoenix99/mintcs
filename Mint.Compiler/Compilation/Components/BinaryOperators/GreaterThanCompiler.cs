namespace Mint.Compilation.Components
{
    internal class GreaterThanCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.GREATER;

        public GreaterThanCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
