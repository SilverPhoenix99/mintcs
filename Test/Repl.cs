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
            var binding = new Closure(new Object());
            for(var i = 1; ; i++)
            {
                var fragment = Prompt($"imt[{i}]> ");

                try
                {
                    var ast = Parser.Parse("(imt)", fragment);

                    if(ast.List.Count == 0)
                    {
                        continue;
                    }

                    DumpAst(ast);
                    var expr = CompileAst(ast, binding);
                    DumpExpression(expr);
                    RunExpression(expr);
                    Console.WriteLine();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
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

        static LambdaExpression CompileAst(Ast<Token> ast, Closure binding)
        {
            var compiler = new Compiler("(imt)", binding);
            var body = ast.Accept(compiler);
            return Expression.Lambda(body);
        }

        internal static void DumpExpression(Expression expr)
        {
            Console.WriteLine(DEBUGVIEW_INFO.Invoke(expr, new object[0]));
            Console.WriteLine();
        }

        static void RunExpression(LambdaExpression expr)
        {
            var lambda = expr.Compile();
            var result = (iObject) lambda.DynamicInvoke();
            Console.WriteLine(result.Inspect());
            Console.WriteLine();
        }
    }
}
