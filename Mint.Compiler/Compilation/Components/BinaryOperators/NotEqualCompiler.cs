namespace Mint.Compilation.Components
{
    internal class NotEqualCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.NEQ;

        public NotEqualCompiler(Compiler compiler) : base(compiler)
        { }
    }
}