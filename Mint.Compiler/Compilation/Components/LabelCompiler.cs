using System.Linq.Expressions;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class LabelCompiler : CompilerComponentBase
    {
        private string Label => Node.Value.Value;

        private Ast<Token> Value => Node[0];

        public LabelCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var left = Label;
            left = left.Remove(left.Length - 1);

            var label = Constant(new Symbol(left), typeof(iObject));
            var value = Value.Accept(Compiler);
            return CompilerUtils.NewArray(label, value);
        }
    }
}
