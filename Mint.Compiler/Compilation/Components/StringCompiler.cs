using Mint.Parse;
using Mint.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class StringCompiler : StringContentCompiler
    {
        protected static readonly ConstructorInfo STRING_CTOR1 = Reflector.Ctor<String>();
        protected static readonly ConstructorInfo STRING_CTOR2 = Reflector.Ctor<String>(typeof(String));
        private static readonly MethodInfo METHOD_STRING_CONCAT = Reflector<String>.Method(_ => _.Concat(null));

        public StringCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile(Ast<Token> ast)
        {
            if(ast.List.Count == 1 && ast[0].Value.Type == tSTRING_CONTENT)
            {
                return Convert(
                    New(
                        STRING_CTOR2,
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
            var list = ast.Select(_ => _.Accept(Compiler)).Select(
                expr => expr.Type == typeof(String) ? expr : Compiler.NewString(expr)
            );

            list = new[] { first }.Concat(list);
            first = list.Aggregate((l, r) => Call(l, METHOD_STRING_CONCAT, r));

            return Convert(first, typeof(iObject));
        }

        protected static IEnumerable<List<Ast<Token>>> GroupWords(IEnumerable<Ast<Token>> list)
        {
            var accumulator = new List<List<Ast<Token>>> { new List<Ast<Token>>() };
            return list.Aggregate(accumulator, AggregateNodes);
        }

        private static List<List<Ast<Token>>> AggregateNodes(List<List<Ast<Token>>> accumulator, Ast<Token> node)
        {
            if(node.Value.Type == tSPACE)
            {
                accumulator.Add(new List<Ast<Token>>());
            }
            else
            {
                accumulator.Last().Add(node);
            }
            return accumulator;
        }
    }
}