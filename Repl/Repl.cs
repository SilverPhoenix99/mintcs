using System;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Compilation;
using Mint.Parse;
using static System.Reflection.BindingFlags;

namespace Mint
{
    internal static class Repl
    {
        private static readonly MethodInfo DEBUGVIEW_INFO =
            typeof(Expression).GetProperty("DebugView", Instance | NonPublic).GetMethod;

        public static void Run()
        {
            var binding = new Closure(new Object());
            for(var i = 1L; ; i++)
            {
                var fragment = Prompt($"imt[{i}]> ");

                if(fragment.Trim() == "quit")
                {
                    break;
                }

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

        private static string Prompt(string prompt = "imt> ")
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        internal static void DumpAst(Ast<Token> ast)
        {
            var doc = AstXmlSerializer.ToXml(ast);
            Console.WriteLine(doc.ToString());
            Console.WriteLine();
        }

        private static Expression<Func<iObject>> CompileAst(Ast<Token> ast, Closure binding)
        {
            var compiler = new Compiler("(imt)", binding, ast);
            var body = compiler.Compile();
            return Expression.Lambda<Func<iObject>>(body);
        }

        internal static void DumpExpression(Expression expr)
        {
            Console.WriteLine(DEBUGVIEW_INFO.Invoke(expr, new object[0]));
            Console.WriteLine();
        }

        private static void RunExpression(Expression<Func<iObject>> expr)
        {
            var lambda = expr.Compile();
            var result = lambda();
            Console.WriteLine(result.Inspect());
            Console.WriteLine();
        }
    }
}
