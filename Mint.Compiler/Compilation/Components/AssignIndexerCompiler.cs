using System.Collections.Generic;
using Mint.Parse;
using Mint.Binding.Arguments;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class AssignIndexerCompiler : IndexerCompiler
    {
        // <left>.[*<args>] = <right>   =>   <left>.[]=(*<args>, <right>)

        protected override Ast<Token> LeftNode => Node[0][0];
        private Ast<Token> RightNode => Node[1];
        protected override Ast<Token> ArgumentsNode => Node[0][1];

        public AssignIndexerCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            base.Shift();
            Push(RightNode);
        }

        public override Expression Reduce()
        {
            var left = Pop();
            IEnumerable<InvocationArgument> arguments = PopArguments();
            var right = Pop();
            var result = Variable(typeof(iObject), "result");

            arguments = arguments.Concat(new[] { new InvocationArgument(ArgumentKind.Simple, result) });

            var visibility = CompilerUtils.GetVisibility(LeftNode);
            var callIndexer = CompilerUtils.Call(left, Symbol.ASET, visibility, arguments.ToArray());

            return Block(
                typeof(iObject),
                new[] { result },
                Assign(result, right),
                callIndexer,
                result
            );
        }
    }
}