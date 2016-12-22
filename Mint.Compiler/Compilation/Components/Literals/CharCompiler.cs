using System.Linq;
using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class CharCompiler : StringContentCompiler
    {
        public CharCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var first = base.Compile();
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