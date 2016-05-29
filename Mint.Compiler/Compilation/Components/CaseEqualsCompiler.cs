namespace Mint.Compilation.Components
{
    internal class CaseEqualsCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.EQQ;

        public CaseEqualsCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
