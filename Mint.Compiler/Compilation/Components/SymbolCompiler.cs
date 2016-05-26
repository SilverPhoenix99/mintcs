using Mint.Parse;
using Mint.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class SymbolCompiler : StringCompiler
    {
        private static readonly ConstructorInfo SYMBOL_CTOR = Reflector.Ctor<Symbol>(typeof(string));

        public SymbolCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile(Ast<Token> ast)
        {
            var content = ast.List[0];

            return content.IsList
                ? Compile(content.List)
                : Constant(new Symbol(content.Value.Value), typeof(iObject));
        }

        protected Expression Compile(IEnumerable<Ast<Token>> content)
        {
            var compiledContent = Compile(New(STRING_CTOR1), content);
            compiledContent = ((UnaryExpression) compiledContent).Operand;
            compiledContent = Convert(compiledContent, typeof(string));
            var symbol = New(SYMBOL_CTOR, compiledContent);
            return Convert(symbol, typeof(iObject));
        }
    }
}