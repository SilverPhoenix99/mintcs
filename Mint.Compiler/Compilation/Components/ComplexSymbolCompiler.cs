using System.Linq;
using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class ComplexSymbolCompiler : StringCompiler
    {
        public ComplexSymbolCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var count = Node.List.Count;
            var contents = Enumerable.Range(0, count).Select(_ => Pop());

            var first = String.Expressions.New();
            var body = CompilerUtils.StringConcat(first, contents);
            body = ((UnaryExpression) body).Operand;
            body = body.Cast<string>();
            var symbol = Symbol.Expressions.New(body);
            return symbol.Cast<iObject>();
        }
    }
}
