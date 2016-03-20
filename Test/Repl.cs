using Mint;
using Mint.Compilation;
using Mint.Parser;
using System;
using System.Linq.Expressions;
using System.Reflection;
using static System.Reflection.BindingFlags;

namespace Test
{
    static class Repl
    {
        public static readonly MethodInfo DEBUGVIEW_INFO =
            typeof(Expression).GetProperty("DebugView", Instance | NonPublic).GetMethod;

        public static void Run()
        {
            var i = 0;
            for(;;)
            {
                var fragment = Prompt($"imt[{++i}]> ");

                try
                {
                    var ast = Parser.Parse(fragment);
                    DumpAst(ast);
                    var expr = ast.Accept(new Compiler("(imt)"));
                    DumpExpression(expr);
                    RunExpression(expr);
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

        static void DumpExpression(Expression expr)
        {
            Console.WriteLine(DEBUGVIEW_INFO.Invoke(expr, new object[0]));
            Console.WriteLine();
        }

        static void RunExpression(Expression expr)
        {
            var lambda = Expression.Lambda(expr).Compile();
            var result = (iObject) lambda.DynamicInvoke();
            Console.WriteLine(result.Inspect());
            Console.WriteLine();
        }
    }
}
