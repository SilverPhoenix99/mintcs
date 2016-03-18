using Mint.Parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using static Mint.Parser.TokenType;

namespace Mint
{
    class Interpreter : AstVisitor<Token, iObject>
    {
        public Interpreter()
        {
            Register(tINTEGER,        ProcessInteger);
            Register(tFLOAT,          ProcessFloat);
            Register(kTRUE,           ProcessTrue);
            Register(kFALSE,          ProcessFalse);
            Register(kNIL,            ProcessNil);
            Register(tSYMBEG,         ProcessSymbol);
            Register(tSTRING_BEG,     ProcessString);
            Register(tSTRING_CONTENT, ProcessStringContent);
            Register(tCHAR,           ProcessChar);
        }

        private Dictionary<TokenType, Func<Ast<Token>, iObject>> Actions { get; } =
            new Dictionary<TokenType, Func<Ast<Token>, iObject>>();

        public void Register(TokenType type, Func<Ast<Token>, iObject> action)
        {
            Actions[type] = action;
        }

        public iObject Visit(Ast<Token> ast)
        {
            if(ast.IsList)
            {
                iObject result = new NilClass();
                foreach(var child in ast.List)
                {
                    result = child.Accept(this);
                }

                return result;
            }

            try
            {
                return Actions[ast.Value.Type](ast);
            }
            catch(KeyNotFoundException e)
            {
                throw new ArgumentException($"Token type {ast.Value.Type} not registered.", "ast", e);
            }
        }

        private Regex CLEAN_INTEGER = new Regex(@"[_BODX]", RegexOptions.Compiled);

        protected iObject ProcessInteger(Ast<Token> ast)
        {
            var tok = ast.Value;
            var str = CLEAN_INTEGER.Replace(tok.Value.ToUpper(), "");
            var num_base = (int) tok.Properties["num_base"];
            var val = Convert.ToInt64(str, num_base);
            return new Fixnum(val);
        }

        protected iObject ProcessFloat(Ast<Token> ast)
        {
            var tok = ast.Value;
            var str = tok.Value.Replace("_", "");
            var val = Convert.ToDouble(str, CultureInfo.InvariantCulture);
            return new Float(val);
        }

        protected iObject ProcessTrue(Ast<Token> ast)
        {
            return new TrueClass();
        }

        protected iObject ProcessFalse(Ast<Token> ast)
        {
            return new FalseClass();
        }

        protected iObject ProcessNil(Ast<Token> ast)
        {
            return new NilClass();
        }

        protected iObject ProcessSymbol(Ast<Token> ast)
        {
            var content = ast.List[0];
            var value = content.Value?.Value ?? ((String) ProcessString(content)).Value;
            return new Symbol(value);
        }

        protected iObject ProcessString(Ast<Token> ast)
        {
            var str = new String();
            ProcessStringInternal(ast, str);
            return str;
        }
        
        protected iObject ProcessStringContent(Ast<Token> ast)
        {
            return new String(ast.Value.Value);
        }
        
        protected iObject ProcessChar(Ast<Token> ast)
        {
            var str = new String(ast.Value.Value);
            ProcessStringInternal(ast, str);
            return str;
        }

        protected void ProcessStringInternal(Ast<Token> ast, String str)
        {
            foreach(var content in ast.List)
            {
                if(!content.IsList)
                {
                    str.Concat( Visit(content) );
                    continue;
                }

                foreach(var subcontent in content.List)
                {
                    str.Concat( Visit(subcontent) );
                }
            }
        }
    }
}
