namespace Mint.Compilation.Components
{
    internal class LessThanCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.LESS;

        public LessThanCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
