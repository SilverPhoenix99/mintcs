using System;
using System.Linq.Expressions;
using Mint.Compilation;
using Mint.MethodBinding;
using Mint.Parse;

namespace Mint
{
    internal static class Repl
    {
        public static void Run()
        {
            var lastResult = new LocalVariable(new Symbol("_"));
            var previousResult = new LocalVariable(new Symbol("__"));
            var binding = new CallFrame(null, new Object());
            binding.AddLocal(lastResult);
            binding.AddLocal(previousResult);

            SyntaxNode node;

            for(var i = 1L; ; i++)
            {
                var fragment = Prompt($"imt[{i}]> ");

                if(fragment == null || fragment.Trim() == "quit")
                {
                    break;
                }

                try
                {
                    for(;;)
                    {
                        node = Parser.ParseString("(imt)", fragment);

                        if(node != null)
                        {
                            break;
                        }

                        fragment += "\n" + Prompt($"imt[{i}]* ");
                    }

                    if(node.List.Count == 0)
                    {
                        continue;
                    }

                    DumpAst(node);
                    var expr = CompileAst(node, binding);
                    DumpExpression(expr);
                    var result = RunExpression(expr);

                    previousResult.Value = lastResult.Value;
                    lastResult.Value = result;

                    Console.WriteLine();
                }
                catch(System.Exception e)
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

        internal static void DumpAst(SyntaxNode node)
        {
            var doc = AstXmlSerializer.ToXml(node);
            Console.WriteLine(doc.ToString());
            Console.WriteLine();
        }

        private static Expression<Func<iObject>> CompileAst(SyntaxNode node, CallFrame topLevelFrame)
        {
            var compiler = new Compiler("(imt)", topLevelFrame);
            var body = compiler.Compile(node);
            return Expression.Lambda<Func<iObject>>(body);
        }

        internal static void DumpExpression(Expression expr)
        {
            Console.WriteLine(expr.Inspect());
            Console.WriteLine();
        }

        private static iObject RunExpression(Expression<Func<iObject>> expr)
        {
            var lambda = expr.Compile();
            var result = lambda();
            Console.WriteLine(result.Inspect());
            Console.WriteLine();
            return result;
        }
    }
}
