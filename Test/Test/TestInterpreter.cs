using Mint;
using Mint.Parser;
using System;

namespace Test
{
    static class TestInterpreter
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

                var val = new Interpreter().Visit(ast);

                Console.WriteLine(val.Inspect());
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
