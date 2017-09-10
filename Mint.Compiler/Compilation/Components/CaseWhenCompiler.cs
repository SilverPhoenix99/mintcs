using System.Collections.Generic;
using System.Linq.Expressions;
using Mint.Parse;
using System.Linq;
using static System.Linq.Expressions.Expression;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class CaseWhenCompiler : CompilerComponentBase
    {
        private SyntaxNode BodyNode => Node[1];

        private IEnumerable<SyntaxNode> WhenNodes => BodyNode.Where(n => n.Token.Type == kWHEN);

        private SyntaxNode ElseNode => BodyNode.LastOrDefault(n => n.Token.Type == kELSE);

        public CaseWhenCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var swithCases = WhenNodes.Select(CompileWhen);
            var defaultCase = CompileElseNode();
            return Switch(typeof(iObject), Constant(true), defaultCase, null, swithCases);
        }

        private SwitchCase CompileWhen(SyntaxNode node)
        {
            var condition = node[0].Accept(Compiler);
            condition = CompilerUtils.ToBool(condition);

            var body = node[1].Accept(Compiler);

            return SwitchCase(body, condition);
        }

        private Expression CompileElseNode() => ElseNode?.Accept(Compiler) ?? NilClass.Expressions.Instance;
    }
}
