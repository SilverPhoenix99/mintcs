using System;
using System.Linq.Expressions;
//using Mint.Compilation;
using Mint.Parse;

namespace Mint.Test
{
    internal static class TestCompiler
    {
        public static void Test(params string[] args)
        {
            /*var fragment = string.Join("\n", args);

            try
            {
                var ast = Parser.Parse("(test compiler)", fragment);

                var doc = AstXmlSerializer.ToXml(ast);
                Console.WriteLine(doc.ToString());
                Console.WriteLine();

                var expr = ast.Accept(new Compiler("(TestCompiler)", new Closure(new Object())));

                Console.WriteLine(Repl.DEBUGVIEW_INFO.Invoke(expr, System.Array.Empty<object>()));
                Console.WriteLine();

                var lambda = Expression.Lambda(expr).Compile();
                Console.WriteLine(lambda.DynamicInvoke());
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                Console.WriteLine();
            }*/
        }
    }
}
