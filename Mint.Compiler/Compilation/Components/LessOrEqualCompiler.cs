namespace Mint.Compilation.Components
{
    internal class LessOrEqualCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.LEQ;

        public LessOrEqualCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
