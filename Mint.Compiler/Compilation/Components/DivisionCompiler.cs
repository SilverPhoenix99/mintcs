namespace Mint.Compilation.Components
{
    internal class DivisionCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.DIV;

        public DivisionCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
