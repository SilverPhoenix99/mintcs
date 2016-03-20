using Mint.Compilation;
using Mint.Parser;
using System;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Test
{
    static class TestCompiler
    {
        public static void Test(params string[] args)
        {
            var fragment = string.Join("\n", args);

            try
            {
                var ast = Parser.Parse(fragment);

                var doc = AstXmlSerializer.ToXml(ast);
                Console.WriteLine(doc.ToString());
                Console.WriteLine();

                var expr = ast.Accept(new Compiler("(TestCompiler)"));

                Console.WriteLine(Repl.DEBUGVIEW_INFO.Invoke(expr, new object[0]));
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
            }
        }
    }
}
