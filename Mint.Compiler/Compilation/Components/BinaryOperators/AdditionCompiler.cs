namespace Mint.Compilation.Components
{
    internal class AdditionCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.PLUS;

        public AdditionCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
