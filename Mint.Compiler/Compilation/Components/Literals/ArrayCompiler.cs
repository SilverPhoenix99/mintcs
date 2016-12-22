using System.Linq;
using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class ArrayCompiler : CompilerComponentBase
    {
        public ArrayCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var elements = Node.Select(_ => _.Accept(Compiler));
            return CompilerUtils.NewArray(elements.ToArray());
        }
    }
}