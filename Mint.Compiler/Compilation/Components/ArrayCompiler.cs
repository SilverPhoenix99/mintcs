using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class ArrayCompiler : CompilerComponentBase
    {
        public ArrayCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var count = Node.List.Count;
            var elements = Enumerable.Range(0, count).Select(_ => Pop());
            var array = CompilerUtils.NewArray(elements.ToArray());
            return array.Cast<iObject>();
        }
    }
}