using Mint.Binding;
using Mint.Binding.Arguments;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class AssignPropertyCompiler : CompilerComponentBase
    {
        // <left>.<name> = <right>   =>   <left>.<name=>(<right>)

        public AssignPropertyCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            Push(Node[0][0]);
            Push(Node[1]);
        }

        public override Expression Reduce()
        {
            var left = Pop();
            var right = Pop();
            var name = new Symbol($"{Node[0][1].Value.Value}=");
            var result = Variable(typeof(iObject), "result");

            var assignResult = Assign(result, right);

            var argument = new InvocationArgument(ArgumentKind.Simple, result);
            var invokeSetter = CompilerUtils.Invoke(Visibility.Public, left, name, argument);

            return Block(
                typeof(iObject), new[] { result },
                assignResult,
                invokeSetter,
                result
            );
        }
    }
}