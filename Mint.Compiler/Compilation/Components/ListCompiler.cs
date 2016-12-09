using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class ListCompiler : CompilerComponentBase
    {
        public ListCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var count = Node.List.Count;

            if(count == 0)
            {
                return NilClass.Expressions.Instance;
            }

            if(count == 1)
            {
                return Pop();
            }

            var body = Enumerable.Range(0, count).Select(_ => Pop());
            return Block(typeof(iObject), body);
        }

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