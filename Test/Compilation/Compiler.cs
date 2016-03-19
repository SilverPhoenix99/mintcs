using Microsoft.CSharp.RuntimeBinder;
using Mint.Extensions;
using Mint.Parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
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
            Register(tSTRING_BEG,     CompileString);
            Register(tCHAR,           CompileChar);
            Register(tSTRING_CONTENT, CompileStringContent);
            Register(tSTRING_DBEG,    CompileList);
        }

        private Dictionary<TokenType, Func<Ast<Token>, Expression>> Actions { get; } =
            new Dictionary<TokenType, Func<Ast<Token>, Expression>>();

        public void Register(TokenType type, Func<Ast<Token>, Expression> action)
        {
            Actions[type] = action;
        }

        public Expression Visit(Ast<Token> ast)
        {
            if(ast.IsList)
            {
                return CompileList(ast);
            }

            Func<Ast<Token>, Expression> action;
            if(Actions.TryGetValue(ast.Value.Type, out action))
            {
                return action(ast);
            }

            throw new ArgumentException($"Token type {ast.Value.Type} not registered.", "ast");
        }

        private static readonly Regex CLEAN_INTEGER = new Regex(@"[_BODX]", RegexOptions.Compiled);

        protected Expression CompileList(Ast<Token> ast)
        {
            return ast.List.Count == 1 ? ast[0].Accept(this) : Block(ast.List.Select(_ => _.Accept(this)));
        }

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
            return Constant(new TrueClass());
        }

        protected Expression CompileFalse(Ast<Token> ast)
        {
            return Constant(new FalseClass());
        }

        protected Expression CompileNil(Ast<Token> ast)
        {
            return Constant(new NilClass());
        }

        protected static readonly ConstructorInfo STRING_CTOR =
            typeof(String).GetConstructor(new[] { typeof(String) });

        protected Expression CompileSymbol(Ast<Token> ast)
        {
            var content = ast.List[0];

            if(!content.IsList)
            {
                return Constant(new Symbol(content.Value.Value));
            }

            var first = New(STRING_CTOR, Constant(new String("")));
            var expr = CompileString(first, content);

            // TODO

            throw new NotImplementedException("Symbol with string content");
        }

        protected Expression CompileString(Ast<Token> ast)
        {
            if(ast.List.Count == 1 && ast[0].Value.Type == tSTRING_CONTENT)
            {
                return New(STRING_CTOR, CompileStringContent(ast[0]));
            }

            var first = New(STRING_CTOR, Constant(new String("")));
            return CompileString(first, ast);
        }

        protected Expression CompileChar(Ast<Token> ast)
        {
            var first = New(STRING_CTOR, CompileStringContent(ast));

            if(ast.List.Count == 0)
            {
                return first;
            }

            return CompileString(first, ast);
        }

        private Expression CompileString(Expression first, IEnumerable<Ast<Token>> ast)
        {
            var tmp = ast.Select(_ => _.Accept(this))
                .Select(_ => _.Type == typeof(String) ? _ : New(STRING_CTOR, Call(_, "ToString", null))).ToArray();

            var list = ast.Select(_ => _.Accept(this))
                .Select(_ => _.IsConstant() ? _ : New(STRING_CTOR, Call(_, "ToString", null)));

            list = new Expression[] { first }.Concat(list);

            return list.Aggregate((l, r) => Call(l, "Concat", null, r));
        }

        protected Expression CompileStringContent(Ast<Token> ast)
        {
            return Constant(new String(ast.Value.Value));
        }

        private CallSiteBinder InvokeMember(string methodName, int numArgs = 0)
        {
            IEnumerable<CSharpArgumentInfo> parameterFlags;

            if(numArgs == 0)
            {
                parameterFlags = new CSharpArgumentInfo[0];
            }
            else
            {
                parameterFlags = Enumerable.Repeat<object>(null, numArgs).Select(
                    _ => CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
                );
            }

            parameterFlags = new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }
                .Concat(parameterFlags);

            return Microsoft.CSharp.RuntimeBinder.Binder
                .InvokeMember(CSharpBinderFlags.None, methodName, null, GetType(), parameterFlags);
        }
    }
}
