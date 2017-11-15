using System.Collections.Generic;
using QUT.Gppg;

namespace Mint.Parse
{
    public class Token
    {
        public TokenType Type { get; }
        public string Text { get; }
        public LexLocation Location { get; }
        public Dictionary<string, object> Properties { get; }

        public Token(TokenType type, string text, LexLocation location)
        {
            Type = type;
            Text = text;
            Location = location;
            Properties = new Dictionary<string, object>();
        }

        public static readonly Token EOF = new Token(TokenType.EOF, null, null);

        public override string ToString()
        {
            var properties = Properties.Count == 0 ? "" : ", **";

            return $"Token({Type}, \"{Text}\", [{Location.StartLine} {Location.StartColumn}]{properties})";
        }

        public void MergeProperties(Token other)
        {
            foreach(var property in other.Properties)
            {
                Properties[property.Key] = property.Value;
            }
        }
    }
}
