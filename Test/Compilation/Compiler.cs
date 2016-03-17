using Mint.Parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using static Mint.Parser.TokenType;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation
{
    public class Compiler : AstVisitor<Token, Expression>
    {
        public Compiler()
        {
            Register(tINTEGER,        CompileInteger);
            Register(tFLOAT,          CompileFloat);
            Register(kTRUE,           CompileTrue);
            Register(kFALSE,          CompileFalse);
            Register(kNIL,            CompileNil);
            Register(tSYMBEG,         CompileSymbol);
        }
        
        private Dictionary<TokenType, Func<Ast<Token>, Expression>> Actions { get; } =
            new Dictionary<TokenType, Func<Ast<Token>, Expression>>();

        public void Register(TokenType type, Func<Ast<Token>, Expression> action)
        {
            Actions[type] = action;
        }

        public Expression Visit(Ast<Token> ast)
        {
            if(ast.Value == null)
            {
                // It's a list
                return Block(ast.List.Select(child => Visit(child)));
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

        protected Expression CompileInteger(Ast<Token> ast)
        {
            var tok = ast.Value;
            var str = CLEAN_INTEGER.Replace(tok.Value.ToUpper(), "");
            var num_base = (int) tok.Properties["num_base"];
            var val = System.Convert.ToInt64(str, num_base);
            return Constant(new Fixnum(val));
        }

        protected Expression CompileFloat(Ast<Token> ast)
        {
            var tok = ast.Value;
            var str = tok.Value.Replace("_", "");
            var val = System.Convert.ToDouble(str, CultureInfo.InvariantCulture);
            return Constant(new Float(val));
        }

        protected Expression CompileTrue(Ast<Token> ast)
        {
            return Constant(new True());
        }

        protected Expression CompileFalse(Ast<Token> ast)
        {
            return Constant(new False());
        }

        protected Expression CompileNil(Ast<Token> ast)
        {
            return Constant(new Nil());
        }

        protected Expression CompileSymbol(Ast<Token> ast)
        {
            var content = ast.List[0];
            var value = content.Value?.Value ;//?? ((Mint.String) ProcessString(content)).Value;
            
            if(value == null) throw new NotImplementedException("Symbol with string content");
            
            return Constant(new Symbol(value));
        }
    }
}
