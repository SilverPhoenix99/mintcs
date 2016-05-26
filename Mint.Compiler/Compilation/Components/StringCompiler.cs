using Mint.Parse;
using Mint.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class StringCompiler : StringContentCompiler
    {
        protected static readonly ConstructorInfo STRING_CTOR1 = Reflector.Ctor<String>();
        private static readonly ConstructorInfo STRING_CTOR2 = Reflector.Ctor<String>(typeof(string));
        protected static readonly ConstructorInfo STRING_CTOR3 = Reflector.Ctor<String>(typeof(String));
        private static readonly MethodInfo METHOD_OBJECT_TOSTRING = Reflector<object>.Method(_ => _.ToString());
        private static readonly MethodInfo METHOD_STRING_CONCAT = Reflector<String>.Method(_ => _.Concat(null));

        public StringCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile(Ast<Token> ast)
        {
            if(ast.List.Count == 1 && ast[0].Value.Type == TokenType.tSTRING_CONTENT)
            {
                return Convert(
                    New(
                        STRING_CTOR3,
                        CompileContent(ast[0])
                    ),
                    typeof(iObject)
                );
            }

            return Compile(New(STRING_CTOR1), ast);
        }

        protected Expression CompileContent(Ast<Token> ast) => base.Compile(ast);

        protected Expression Compile(Expression first, IEnumerable<Ast<Token>> ast)
        {
            var list = ast.Select(_ => _.Accept(Compiler)).Select(_ => _.Type == typeof(String) ? _ : NewString(_));

            list = new[] { first }.Concat(list);
            first = list.Aggregate((l, r) => Call(l, METHOD_STRING_CONCAT, r));

            return Convert(first, typeof(iObject));
        }

        private Expression NewString(Expression argument)
        {
            var call = Call(Convert(argument, typeof(object)), METHOD_OBJECT_TOSTRING, null);
            return New(STRING_CTOR2, call);
        }
    }
}