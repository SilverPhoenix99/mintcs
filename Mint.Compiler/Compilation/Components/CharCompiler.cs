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
            var first = Constant(ReduceContent());
            var count = Node.List.Count;

            if(count == 0)
            {
                return first.Cast<iObject>();
            }

            var contents = Enumerable.Range(0, count).Select(_ => Pop());
            return CompilerUtils.StringConcat(first, contents);
        }
    }
}