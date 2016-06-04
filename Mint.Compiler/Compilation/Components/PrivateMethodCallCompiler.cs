using System.Linq;
using System.Linq.Expressions;
using Mint.Binding;
using Mint.Binding.Arguments;
using Mint.Parse;
using static Mint.Parse.TokenType;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class PrivateMethodCallCompiler : CompilerComponentBase
    {
        protected string MethodName => Node[1].Value.Value;
        private Ast<Token> ArgumentsNode => Node[2];

        public PrivateMethodCallCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            var blockNode = ArgumentsNode.FirstOrDefault(
                _ => _.Value.Type == kDO || _.Value.Type == kLBRACE2
            );

            if(ArgumentsNode.Any(_ => _.Value.Type == kAMPER) && blockNode != null)
            {
                var line = blockNode.Value.Location.StartLine;
                throw new SyntaxError(Compiler.Filename, line, "both block arg and actual block given");
            }

            foreach(var argument in ArgumentsNode)
            {
                Push(argument);
            }
        }

        public override Expression Reduce()
        {
            var instance = GetLeftExpression();
            var arguments = PopArguments();
            var methodName = new Symbol(MethodName);
            var visibility = GetVisibility();

            return CompilerUtils.Call(instance, methodName, visibility, arguments);
        }

        protected virtual Expression GetLeftExpression() =>
            Constant(Compiler.CurrentScope.Closure.Self, typeof(iObject));

        protected virtual Visibility GetVisibility() => Visibility.Private;

        protected InvocationArgument[] PopArguments()
        {
            var count = ArgumentsNode.List.Count;
            var arguments = Enumerable.Range(0, count).Select(_ => Pop());
            var types = ArgumentsNode.Select(_ => _.Value.Type);

            return types.Zip(arguments, CreateInvocationArgument).ToArray();
        }

        private static InvocationArgument CreateInvocationArgument(TokenType type, Expression argument) =>
            new InvocationArgument(CompilerUtils.GetArgumentKind(type), argument);


    }
}
