using System.Linq;
using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class ComplexSymbolCompiler : StringCompiler
    {
        public ComplexSymbolCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var first = String.Expressions.New();
            var contents = Node.Select(_ => _.Accept(Compiler));

            var body = CompilerUtils.StringConcat(first, contents);
            body = ((UnaryExpression) body).Operand;
            body = body.Cast<string>();

            var symbol = Symbol.Expressions.New(body);
            return symbol.Cast<iObject>();
        }
    }
}
