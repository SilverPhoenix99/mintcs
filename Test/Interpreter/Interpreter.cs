using Mint.Parser;
using Mint.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using static Mint.Parser.TokenType;

namespace Test.Interpreter
{
    class Interpreter : AstVisitor<Token, iObject>
    {
        public Interpreter()
        {
            Register(tINTEGER, ProcessInteger);
            Register(tFLOAT,   ProcessFloat);
            Register(kTRUE,    ProcessTrue);
            Register(kFALSE,   ProcessFalse);
            Register(kNIL,     ProcessNil);
            Register(tSYMBEG,  ProcessSymbol);
            //Register(tSTRING_BEG, ProcessString);
        }

        private Dictionary<TokenType, Func<Ast<Token>, iObject>> Actions { get; } =
            new Dictionary<TokenType, Func<Ast<Token>, iObject>>();

        public void Register(TokenType type, Func<Ast<Token>, iObject> action)
        {
            Actions[type] = action;
        }

        public iObject Visit(Ast<Token> ast)
        {
            if(ast.Value == null)
            {
                // It's a list

                iObject result = new Nil();
                foreach(var child in ast.List)
                {
                    result = Visit(child);
                }

                return result;
            }

            return Actions[ast.Value.Type](ast);
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
            return new True();
        }

        protected iObject ProcessFalse(Ast<Token> ast)
        {
            return new False();
        }

        protected iObject ProcessNil(Ast<Token> ast)
        {
            return new Nil();
        }

        protected iObject ProcessSymbol(Ast<Token> ast)
        {
            var tok = ast.Value;
            var content = ast.List[0].Value;

            if(content == null)
            {
                // TODO: process string contents
                throw new NotImplementedException();
            }

            return new Symbol(content.Value);
        }
    }
}
