using System.Xml.Linq;
using Mint.Parse;

namespace Mint
{
    public class AstXmlSerializer : AstVisitor
    {
        private XContainer current;

        public AstXmlSerializer(SyntaxNode node)
        {
            Node = node;
            current = Document = new XDocument();
        }

        public SyntaxNode Node { get; }
        public XDocument Document { get; }

        public XDocument Visit()
        {
            Node.Accept(this);
            return Document;
        }

        public void Visit(SyntaxNode node)
        {
            var token = node.Token;
            var prev = current;

            if(token == null)
            {
                current = new XElement(current == Document ? "ast" : "list");
            }
            else
            {
                current = new XElement("token",
                    new XAttribute("type", token.Type),
                    new XAttribute("location", $"{token.Location.StartLine} {token.Location.StartColumn}"),
                    new XAttribute("text", token.Text)
                );

                foreach(var property in token.Properties)
                {
                    current.Add(new XElement("property",
                        new XAttribute("key",   property.Key),
                        new XAttribute("value", property.Value)
                    ));
                }
            }

            prev.Add(current);

            foreach(var ast in node.List)
            {
                ast.Accept(this);
            }

            current = prev;
        }

        public static XDocument ToXml(SyntaxNode node)
        {
            var visitor = new AstXmlSerializer(node);
            return visitor.Visit();
        }
    }
}
