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
        protected string MethodName => Node[1].Token.Text;

        private SyntaxNode ArgumentsNode => Node[2];

        // remove empty double splats: `**{}`
        private IEnumerable<SyntaxNode> Arguments => ArgumentsNode.Where(
            arg => arg.Token.Type != kDSTAR || arg[0].Token.Type != kLBRACE || arg[0].List.Count != 0
        );

        public PrivateMethodCallCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var blockNode = ArgumentsNode.FirstOrDefault(_ => _.Token.Type == kDO || _.Token.Type == kLBRACE2);

            if(blockNode != null && ArgumentsNode.Any(_ => _.Token.Type == kAMPER))
            {
                var line = blockNode.Token.Location.StartLine;
                throw new SyntaxError(Compiler.Filename, line, "both block arg and actual block given");
            }

            var instance = GetLeftExpression();
            var arguments = CompileArguments();
            var methodName = new Symbol(MethodName);
            var visibility = GetVisibility();

            return CompilerUtils.Call(instance, methodName, visibility, arguments);
        }

        protected InvocationArgument[] CompileArguments() => Arguments.Select(CreateInvocationArgument).ToArray();

        private InvocationArgument CreateInvocationArgument(SyntaxNode argument)
        {
            var kind = argument.Token.Type.GetArgumentKind();
            var expression = argument.Accept(Compiler);
            return new InvocationArgument(kind, expression);
        }

        protected virtual Expression GetLeftExpression() => Compiler.CurrentScope.Instance;

        protected virtual Visibility GetVisibility() => Visibility.Private;

        private static InvocationArgument CreateInvocationArgument(TokenType type, Expression argument) =>
            new InvocationArgument(type.GetArgumentKind(), argument);
    }
}
