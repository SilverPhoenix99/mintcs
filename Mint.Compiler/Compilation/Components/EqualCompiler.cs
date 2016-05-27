namespace Mint.Compilation.Components
{
    internal class EqualCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.EQ;

        public EqualCompiler(Compiler compiler) : base(compiler)
        { }
    }
}