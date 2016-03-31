using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Compilation;
using Mint.Parse;
using static System.Reflection.BindingFlags;

namespace Mint
{
    internal static class Repl
    {
        public static readonly MethodInfo DEBUGVIEW_INFO =
            typeof(Expression).GetProperty("DebugView", Instance | NonPublic).GetMethod;

        public static void Run()
        {
            var main = new Object();
            var binding = new Dictionary<Symbol, iObject>();
            for(var i = 1; ; i++)
            {
                var fragment = Prompt($"imt[{i}]> ");

                try
                {
                    var ast = Parser.Parse("(imt)", fragment);
                    DumpAst(ast);
                    var expr = CompileAst(ast, binding);
                    DumpExpression(expr);
                    RunExpression(expr, main);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    Console.WriteLine();
                }
            }
        }

        static string Prompt(string prompt = "imt> ")
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        static void DumpAst(Ast<Token> ast)
        {
            var doc = AstXmlSerializer.ToXml(ast);
            Console.WriteLine(doc.ToString());
            Console.WriteLine();
        }

        static LambdaExpression CompileAst(Ast<Token> ast, IDictionary<Symbol, iObject> binding)
        {
            var compiler = new Compiler("(imt)");
            var body = ast.Accept(compiler);

            var locals =
                from local in compiler.CurrentScope.Variables
                where local.Key != Symbol.SELF
                select local.Value;

            var bindingConstant = Expression.Constant(binding);
            var propertyInfo = binding.GetType().GetProperty("Item");
            Func<Symbol, Expression> property = key =>
                Expression.MakeIndex(
                    bindingConstant,
                    propertyInfo,
                    new[] { Expression.Constant(key) }
                );

            var assignLocals =
                from local in compiler.CurrentScope.Variables
                where local.Key != Symbol.SELF
                select Expression.Assign(local.Value, property(local.Key));

            var assignBinding =
                from local in compiler.CurrentScope.Variables
                where local.Key != Symbol.SELF
                select Expression.Assign(property(local.Key), local.Value);

            return Expression.Lambda(
                Expression.Block(
                    locals,
                    assignLocals.Concat(new [] { body }).Concat(assignBinding)
                ),
                compiler.CurrentScope.Variables[Symbol.SELF]
            );
        }

        static void DumpExpression(Expression expr)
        {
            Console.WriteLine(DEBUGVIEW_INFO.Invoke(expr, new object[0]));
            Console.WriteLine();
        }

        static void RunExpression(LambdaExpression expr, iObject self)
        {
            var lambda = expr.Compile();
            var result = (iObject) lambda.DynamicInvoke(self);
            Console.WriteLine(result.Inspect());
            Console.WriteLine();
        }
    }
}
