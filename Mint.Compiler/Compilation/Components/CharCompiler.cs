using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class CharCompiler : StringCompiler
    {
        public CharCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var first = Constant(CompileContent());
            var count = Node.List.Count;

            if(count == 0)
            {
                return first.Cast<iObject>();
            }

            var contents = Node.Select(_ => _.Accept(Compiler));
            return CompilerUtils.StringConcat(first, contents);
        }
    }
}