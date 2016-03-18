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

                var expr = new Compiler().Visit(ast);

                doc = new XDocument();
                expr.XmlDump(doc);
                Console.WriteLine(doc.ToString());
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

        static void XmlDump(this Expression expr, XContainer element)
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

            if(expr is MethodCallExpression)
            {
                ((MethodCallExpression) expr).XmlDump(element);
                return;
            }

            if(expr is NewExpression)
            {
                ((NewExpression) expr).XmlDump(element);
                return;
            }

            throw new NotImplementedException(expr.GetType().ToString());
        }

        static void XmlDump(this BlockExpression expr, XContainer root)
        {
            XElement element;

            root.Add(element = new XElement("Block"));
            foreach(var node in expr.Expressions)
            {
                node.XmlDump(element);
            }
        }

        static void XmlDump(this ConstantExpression expr, XContainer root)
        {
            root.Add( new XElement("Constant",
                new XAttribute("Type", expr.Value.GetType().ToString()),
                expr.Value
            ));
        }

        static void XmlDump(this MethodCallExpression expr, XContainer root)
        {
            XElement element;

            root.Add(element = new XElement("MethodCall",
                new XAttribute("Method", expr.Method.Name),
                new XAttribute("DeclaringType", expr.Method.DeclaringType.FullName)
            ));

            if(expr.Object != null)
            {
                var obj = new XElement("Object");
                element.Add(obj);
                expr.Object.XmlDump(obj);
            }

            var args = new XElement("Arguments");
            element.Add(args);
            foreach(var argExpr in expr.Arguments)
            {
                argExpr.XmlDump(args);
            }
        }

        static void XmlDump(this NewExpression expr, XContainer root)
        {
            XElement element;

            root.Add(element = new XElement("New",
                new XAttribute("DeclaringType", expr.Constructor.DeclaringType.FullName)
            ));

            foreach(var argExpr in expr.Arguments)
            {
                argExpr.XmlDump(element);
            }
        }
    }
}
