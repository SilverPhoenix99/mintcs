using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class CharCompiler : StringCompiler
    {
        public CharCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var first = Constant(ReduceContent(Node));
            var count = Node.List.Count;

            if(count == 0)
            {
                return Convert(first, typeof(iObject));
            }

            var contents = Enumerable.Range(0, Node.List.Count).Select(_ => Pop());
            return Reduce(first, contents);
        }
    }
}