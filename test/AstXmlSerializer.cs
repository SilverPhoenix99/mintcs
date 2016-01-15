using mint.Compiler;
using System.Xml.Linq;

namespace mint.test
{
    class AstXmlSerializer : AstVisitor<Token>
    {
        private XContainer current;

        public AstXmlSerializer(Ast<Token> ast)
        {
            Ast = ast;
            current = Document = new XDocument();
        }

        public Ast<Token> Ast { get; }
        public XDocument Document { get; }

        public XDocument Visit()
        {
            Ast.Accept(this);
            return Document;
        }

        public void Visit(Ast<Token> node)
        {
            var token = node.Value;
            var prev = current;

            if(token == null)
            {
                current = new XElement(current == Document ? "ast" : "list");
            }
            else
            {
                current = new XElement("token",
                    new XAttribute("type", token.Type),
                    new XAttribute("location", $"{token.Location.Item1} {token.Location.Item2}"),
                    new XAttribute("text", token.Value)
                );

                foreach(var property in token.Properties)
                {
                    current.Add(new XElement("property", new XAttribute(property.Key, property.Value)));
                }
            }

            prev.Add(current);

            foreach(var ast in node.List)
            {
                ast.Accept(this);
            }

            current = prev;
        }

        public static XDocument ToXml(Ast<Token> ast)
        {
            var visitor = new AstXmlSerializer(ast);
            return visitor.Visit();
        }
    }
}
