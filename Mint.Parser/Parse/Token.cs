﻿using System;
using System.Collections.Generic;

namespace Mint.Parse
{
    public class Token
    {
		public TokenType Type                        { get; }
        public string Value                          { get; }
        public Tuple<int, int> Location              { get; } // < line, column >
        public Dictionary<string, object> Properties { get; }

        public Token(TokenType type, string token, Tuple<int, int> location)
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

            return $"[{Type}, \"{Value}\", {Location.Item1}, {Location.Item2}{properties}]";
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
