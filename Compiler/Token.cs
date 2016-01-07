using System;
using System.Collections.Generic;

namespace mint.Compiler
{
    public class Token
    {
		public TokenType Type                        { get; }
        public string Value                          { get; }
        public Tuple<int, int> Location              { get; }
        public Dictionary<string, object> Properties { get; }

        public Token(TokenType type, string token, Tuple<int, int> location)
		{
			Type = type;
            Value = token;
            Location = location;
            Properties = new Dictionary<string, object>();
		}

		public static readonly Token Null = new Token(TokenType.None, null, null);
    }
}
