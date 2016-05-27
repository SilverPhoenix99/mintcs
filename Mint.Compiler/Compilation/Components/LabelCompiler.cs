using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class LabelCompiler : CompilerComponentBase
    {
        public LabelCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var label = Node.Value.Value;
            label = label.Substring(0, label.Length - 1);
            return Constant(new Symbol(label), typeof(iObject));
        }
    }
}
