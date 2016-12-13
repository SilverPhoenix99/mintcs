namespace Mint.Compilation.Components
{
    internal class ModulusCompiler : BinaryOperatorCompiler
    {
        protected override Symbol Operator => Symbol.PERCENT;

        public ModulusCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
