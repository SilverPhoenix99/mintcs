using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mint.MethodBinding;
using Mint.MethodBinding.Arguments;
using Mint.Parse;
using static System.Linq.Expressions.Expression;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class CaseWhenWithValueCompiler : CompilerComponentBase
    {
        private SyntaxNode ValueNode => Node[0];

        private SyntaxNode BodyNode => Node[1];

        private IEnumerable<SyntaxNode> WhenNodes => BodyNode.Where(n => n.Token.Type == kWHEN);

        private SyntaxNode ElseNode => BodyNode.LastOrDefault(n => n.Token.Type == kELSE);

        public CaseWhenWithValueCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var caseValue = ValueNode.Accept(Compiler);
            var callSite = new CallSite(Symbol.EQQ, Visibility.Public, ArgumentKind.Simple);

            var swithCases = WhenNodes.Select(n => CompileWhen(n, caseValue, callSite));
            var defaultCase = CompileElseNode();
            return Switch(typeof(iObject), Constant(true), defaultCase, null, swithCases);
        }

        private SwitchCase CompileWhen(SyntaxNode node, Expression caseValue, CallSite callSite)
        {
            var instance = node[0].Accept(Compiler);
            var arguments = NewArrayInit(typeof(iObject), caseValue);
            Expression condition = CallSite.Expressions.Call(Constant(callSite), instance, arguments);
            condition = CompilerUtils.ToBool(condition);

            var body = node[1].Accept(Compiler);

            return SwitchCase(body, condition);
        }

        private Expression CompileElseNode() => ElseNode?.Accept(Compiler) ?? NilClass.Expressions.Instance;
    }
}