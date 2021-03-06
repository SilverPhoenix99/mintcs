using System.Linq;
using System.Linq.Expressions;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class StringCompiler : StringContentCompiler
    {
        public StringCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            foreach(var child in Node.Where(_ => _.Token?.Type == tSTRING_CONTENT))
            {
                // copy dedent value to children if dedents property is set in Node
                child.Token.MergeProperties(Node.Token);
            }

            if(IsSimpleContent())
            {
                return Node[0].Accept(Compiler);
            }

            var count = Node.List.Count;
            var contents = Node.Select(_ => _.Accept(Compiler));
            return CompilerUtils.StringConcat(String.Expressions.New(), contents);
        }

        private bool IsSimpleContent()
        {
            var hasSingleChild = Node.List.Count == 1;
            var firstChild = Node[0];
            return hasSingleChild && firstChild.Token.Type == tSTRING_CONTENT;
        }
    }
}