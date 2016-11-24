using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mint.MethodBinding;
using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class PrivateMethodCallCompiler : CompilerComponentBase
    {
        protected string MethodName => Node[1].Value.Value;

        private Ast<Token> ArgumentsNode => Node[2];

        // remove empty double splats: `**{}`
        private IEnumerable<Ast<Token>> Arguments => ArgumentsNode.Where(
            arg => arg.Value.Type != kDSTAR || arg[0].Value.Type != kLBRACE || arg[0].List.Count != 0
        );

        public PrivateMethodCallCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            var blockNode = ArgumentsNode.FirstOrDefault(_ => _.Value.Type == kDO || _.Value.Type == kLBRACE2);

            if(blockNode != null && ArgumentsNode.Any(_ => _.Value.Type == kAMPER))
            {
                var line = blockNode.Value.Location.StartLine;
                throw new SyntaxError(Compiler.Filename, line, "both block arg and actual block given");
            }

            foreach(var argument in Arguments)
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

        protected virtual Expression GetLeftExpression() => Compiler.CurrentScope.Instance;

        protected virtual Visibility GetVisibility() => Visibility.Private;

        protected InvocationArgument[] PopArguments()
        {
            var count = Arguments.Count();
            var arguments = Enumerable.Range(0, count).Select(_ => Pop());
            var types = Arguments.Select(_ => _.Value.Type);

            return types.Zip(arguments, CreateInvocationArgument).ToArray();
        }

        private static InvocationArgument CreateInvocationArgument(TokenType type, Expression argument) =>
            new InvocationArgument(type.GetArgumentKind(), argument);
    }
}
