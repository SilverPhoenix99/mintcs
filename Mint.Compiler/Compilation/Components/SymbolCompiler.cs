using Mint.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class SymbolCompiler : StringCompiler
    {
        private static readonly ConstructorInfo SYMBOL_CTOR = Reflector.Ctor<Symbol>(typeof(string));

        public SymbolCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            if(IsIdentifierSymbol())
            {
                return;
            }

            base.Shift();
        }
        
        public override Expression Reduce()
        {
            if(IsIdentifierSymbol())
            {
                return Constant(new Symbol(Node[0].Value.Value), typeof(iObject));
            }

            var count = Node.List.Count;
            var contents = Enumerable.Range(0, count).Select(_ => Pop());

            var first = CompilerUtils.NewString();
            var body = Reduce(first, contents);
            body = ((UnaryExpression) body).Operand;
            body = Convert(body, typeof(string));
            var symbol = New(SYMBOL_CTOR, body);
            return Convert(symbol, typeof(iObject));
        }

        private bool IsIdentifierSymbol() => Node.List.Count == 1 && Node[0].Value.Type == tIDENTIFIER;
    }
}