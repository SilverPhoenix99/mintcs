using Microsoft.CSharp.RuntimeBinder;
using Mint.Extensions;
using Mint.Parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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
            Register(tSTRING_CONTENT, CompileStringContent);
            Register(tSTRING_BEG,     CompileString);
            //Register(tCHAR,           CompileChar);
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
                var list = ast.List.Select(_ => _.Accept(this));
                return list.Count() == 1 ? list.First() : Block(list);
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

        protected Expression CompileSymbol(Ast<Token> ast)
        {
            var content = ast.List[0];

            if(!content.IsList)
            {
                return Constant(new Symbol(content.Value.Value));
            }

            var list = content.List.Select(_ => _.Accept(this));
            var first = list.First();

            if(!first.IsConstant<String>())
            {
                // prepend an empty string
                list = new[] { Constant(new String()) }.Concat(list);
            }
            else if(list.SingleOrDefault() != null) // if list.Count == 1
            {
                var constant = (ConstantExpression) first;
                return Constant(new Symbol(constant.Value.ToString()));
            }

            list.Select(e =>
                (e.IsConstant<Fixnum>())
                ? (Expression) Call(e, e.Type.GetMethod("ToString", new Type[0]))
                : Dynamic( InvokeMember("to_s"), typeof(iObject) )
            );

            // TODO

            throw new NotImplementedException("Symbol with string content");
        }

        protected Expression CompileString(Ast<Token> ast)
        {
            /*var list = ast.List.SelectMany(_ => _.Accept(this));

            list = list.SelectMany(_ => (IEnumerable<Expression>) (
                 (_ as BlockExpression)?.Expressions ?? new Expression[] { _ }
            ));*/

            throw new NotImplementedException("CompileString");
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

            return Binder.InvokeMember(CSharpBinderFlags.None, methodName, null, GetType(), parameterFlags);
        }
    }
}
