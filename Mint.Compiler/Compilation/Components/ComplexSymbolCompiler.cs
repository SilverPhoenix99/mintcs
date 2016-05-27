using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class ComplexSymbolCompiler : StringCompiler
    {
        private static readonly ConstructorInfo SYMBOL_CTOR = Reflector.Ctor<Symbol>(typeof(string));

        public ComplexSymbolCompiler(Compiler compiler) : base(compiler)
        { }
        
        public override Expression Reduce()
        {
            var count = Node.List.Count;
            var contents = Enumerable.Range(0, count).Select(_ => Pop());

            var first = CompilerUtils.NewString();
            var body = Reduce(first, contents);
            body = ((UnaryExpression) body).Operand;
            body = Convert(body, typeof(string));
            var symbol = New(SYMBOL_CTOR, body);
            return Convert(symbol, typeof(iObject));
        }
    }
}
