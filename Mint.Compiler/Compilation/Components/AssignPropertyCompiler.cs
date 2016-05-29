using Mint.Binding.Arguments;
using Mint.Parse;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class AssignPropertyCompiler : CompilerComponentBase
    {
        // <left>.<name> = <right>   =>   <left>.<name=>(<right>)

        private Ast<Token> LeftNode => Node[0][0];
        private Ast<Token> RightNode => Node[1];
        private string PropertyName => Node[0][1].Value.Value;

        public AssignPropertyCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            Push(LeftNode);
            Push(RightNode);
        }

        public override Expression Reduce()
        {
            var left = Pop();
            var right = Pop();
            var result = Variable(typeof(iObject), "result");

            var name = new Symbol($"{PropertyName}=");
            var visibility = GetVisibility(LeftNode);
            var argument = new InvocationArgument(ArgumentKind.Simple, result);
            var callSetter = CompilerUtils.Call(left, name, visibility, argument);

            return Block(
                typeof(iObject),
                new[] { result },
                Assign(result, right),
                callSetter,
                result
            );
        }
    }
}