using System.Collections.Generic;
using QUT.Gppg;

namespace Mint.Parse
{
    public class Token
    {
		public TokenType Type { get; }
        public string Value { get; }
        public LexLocation Location { get; }
        public Dictionary<string, object> Properties { get; }

        public Token(TokenType type, string token, LexLocation location)
		{
			Type = type;
            Value = token;
            Location = location;
            Properties = new Dictionary<string, object>();
		}

		public static readonly Token EOF = new Token(TokenType.EOF, null, null);

        public override string ToString()
        {
            var properties = Properties.Count == 0 ? "" : ", **";

            return $"[{Type}, \"{Value}\", {Location.StartLine}, {Location.StartColumn}{properties}]";
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
