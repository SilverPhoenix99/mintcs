using System.Linq;
using System.Linq.Expressions;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class StringCompiler : StringContentCompiler
    {
        public StringCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            foreach(var child in Node.List)
            {
                if(child.Value?.Type == tSTRING_CONTENT)
                {
                    // Shift: copy dedent value to children if dedents property is set in Node
                    child.Value.MergeProperties(Node.Value);
                }
                Push(child);
            }
        }

        public override Expression Reduce()
        {
            if(IsSimpleContent())
            {
                return Pop();
            }

            var count = Node.List.Count;
            var contents = Enumerable.Range(0, count).Select(_ => Pop());
            return CompilerUtils.StringConcat(CompilerUtils.NewString(), contents);
        }

        private bool IsSimpleContent()
        {
            var hasSingleChild = Node.List.Count == 1;
            var firstChild = Node[0];
            return hasSingleChild && firstChild.Value.Type == tSTRING_CONTENT;
        }
    }
}