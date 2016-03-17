using Mint;
using Mint.Compilation;
using Mint.Parser;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Xml.Linq;
using static System.Linq.Expressions.Expression;

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

                var expr = new Compiler().Visit(ast);
                
                doc = new XDocument(new XElement("Expression"));
                expr.XmlDump(doc.Root);
                Console.WriteLine(doc.ToString());
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
        
        static void XmlDump(this Expression expr, XElement element)
        {
            if(expr is BlockExpression)
            {
                ((BlockExpression) expr).XmlDump(element);
                return;
            }
        
            if(expr is ConstantExpression)
            {
                ((ConstantExpression) expr).XmlDump(element);
                return;
            }
        
            /*if(expr is LambdaExpression)
            {
                ((LambdaExpression) expr).XmlDump(element);
                return;
            }
            
            if(expr is BinaryExpression)
            {
                ((BinaryExpression) expr).XmlDump(element);
                return;
            }*/
            
            throw new NotImplementedException(expr.GetType().ToString());
        }
        
        static void XmlDump(this BlockExpression expr, XElement element)
        {
            element.Add(element = new XElement("Block"));
            foreach(var node in expr.Expressions)
            {
                node.XmlDump(element);
            }
        }
        
        static void XmlDump(this ConstantExpression expr, XElement element)
        {
            element.Add( new XElement("Constant",
                new XAttribute("Type", expr.Value.GetType().ToString()),
                expr.Value)
            );
        }
    }
}
