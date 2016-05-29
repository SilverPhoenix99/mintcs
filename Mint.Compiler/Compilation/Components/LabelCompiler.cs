using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class LabelCompiler : CompilerComponentBase
    {
        private string Label => Node.Value.Value;

        public LabelCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var left = Label;
            left = left.Substring(0, left.Length - 1);

            var label = Constant(new Symbol(left), typeof(iObject));
            var value = Pop();
            return CompilerUtils.NewArray(label, value);
        }
    }
}
