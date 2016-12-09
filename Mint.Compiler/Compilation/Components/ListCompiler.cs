using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class ListCompiler : CompilerComponentBase
    {
        public ListCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            switch(Node.List.Count)
            {
                case 0:
                    return NilClass.Expressions.Instance;

                case 1:
                    return Node[0].Accept(Compiler);

                default:
                {
                    var body = Node.Select(_ => _.Accept(Compiler));
                    return Block(typeof(iObject), body);
                }
            }
        }
    }
}