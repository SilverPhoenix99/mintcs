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
        private readonly Dictionary<TokenType, Func<Ast<Token>, Expression>> actions =
            new Dictionary<TokenType, Func<Ast<Token>, Expression>>();
        protected readonly string filename;
        protected readonly Stack<GotoExpression> breakExpressions = new Stack<GotoExpression>();
        protected readonly Stack<Dictionary<string, ParameterExpression>> locals =
            new Stack<Dictionary<string, ParameterExpression>>();

        public Compiler(string filename)
        {
            this.filename = filename;
            locals.Push(new Dictionary<string, ParameterExpression>());

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
            Register(kUMINUS_NUM,     CompileUMinusNum);
            Register(kIF,             CompileIf);
            Register(kIF_MOD,         CompileIf);
            Register(kELSIF,          CompileIf);
            Register(kELSE,           CompileList);
            Register(kUNLESS,         CompileIf);
            Register(kUNLESS_MOD,     CompileIf);
            Register(kQMARK,          CompileQMark);
            Register(kNOT,            CompileNot);
            Register(kAND,            CompileAnd);
            Register(kOR,             CompileOr);
            Register(k__LINE__,       CompileLineNumber);
            Register(k__FILE__,       CompileFileName);
            Register(kWHILE,          CompileWhile);
            Register(kWHILE_MOD,      CompileWhile);
            Register(kUNTIL,          CompileWhile);
            Register(kUNTIL_MOD,      CompileWhile);
        }

        public void Register(TokenType type, Func<Ast<Token>, Expression> action)
        {
            actions[type] = action;
        }

        public Expression Visit(Ast<Token> ast)
        {
            if(ast.IsList)
            {
                return CompileList(ast);
            }

            Func<Ast<Token>, Expression> action;
            if(actions.TryGetValue(ast.Value.Type, out action))
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

        protected Expression CompileTrue(Ast<Token> ast = null)
        {
            return Constant(new TrueClass());
        }

        protected Expression CompileFalse(Ast<Token> ast = null)
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
            
            return New(SYMBOL_CTOR, Call(CompileString(New(STRING_CTOR1), content), "ToString", null));
        }

        protected Expression CompileString(Ast<Token> ast)
        {
            if(ast.List.Count == 1 && ast[0].Value.Type == tSTRING_CONTENT)
            {
                return New(STRING_CTOR3, CompileStringContent(ast[0]));
            }
            
            return CompileString(New(STRING_CTOR1), ast);
        }

        protected Expression CompileChar(Ast<Token> ast)
        {
            var first = New(STRING_CTOR3, CompileStringContent(ast));

            if(ast.List.Count == 0)
            {
                return first;
            }

            return CompileString(first, ast);
        }

        private Expression CompileString(Expression first, IEnumerable<Ast<Token>> ast)
        {
            var list = ast.Select(_ => _.Accept(this))
                .Select(_ => _.Type == typeof(String) ? _ : New(STRING_CTOR2, Call(_, "ToString", null)));

            list = new Expression[] { first }.Concat(list);

            return list.Aggregate((l, r) => Call(l, "Concat", null, r));
        }

        protected Expression CompileStringContent(Ast<Token> ast)
        {
            return Constant(new String(ast.Value.Value));
        }

        protected Expression CompileUMinusNum(Ast<Token> ast)
        {
            return Negate(ast[0].Accept(this));
        }

        protected Expression CompileIf(Ast<Token> ast)
        {
            Expression condition = ToBool(Convert(ast[0].Accept(this), typeof(iObject)));

            if(ast.Value.Type == kUNLESS || ast.Value.Type == kUNLESS_MOD)
            {
                condition = Not(condition);
            }

            var ifTrue = Convert(ast[1].Accept(this), typeof(iObject));

            var ifFalse = ast.List.Count < 3 || ast[2].List.Count == 0
                        ? DFT_ELSE
                        : Convert(ast[2].Accept(this), typeof(iObject));

            return Condition(condition, ifTrue, ifFalse);
        }

        protected Expression CompileQMark(Ast<Token> ast)
        {
            var condition = ToBool(Convert(ast[0].Accept(this), typeof(iObject)));
            var ifTrue    = Convert(ast[1].Accept(this), typeof(iObject));
            var ifFalse   = Convert(ast[2].Accept(this), typeof(iObject));
            return Condition(condition, ifTrue, ifFalse);
        }

        protected Expression CompileNot(Ast<Token> ast)
        {
            var cond = Convert(ast[0].Accept(this), typeof(iObject));
            return Condition(
                ToBool(cond),
                Convert(CompileFalse(), typeof(iObject)),
                Convert(CompileTrue(), typeof(iObject))
            );
        }

        protected Expression CompileAnd(Ast<Token> ast)
        {
            var left  = Convert(ast[0].Accept(this), typeof(iObject));
            var right = Convert(ast[1].Accept(this), typeof(iObject));
            return Condition(ToBool(left), right, left);
        }

        protected Expression CompileOr(Ast<Token> ast)
        {
            var left = Convert(ast[0].Accept(this), typeof(iObject));
            var right = Convert(ast[1].Accept(this), typeof(iObject));
            return Condition(ToBool(left), left, right);
        }

        protected Expression CompileLineNumber(Ast<Token> ast)
        {
            return Constant(new Fixnum(ast.Value.Location.Item1));
        }

        protected Expression CompileFileName(Ast<Token> ast)
        {
            return Constant(new String(filename));
        }

        protected Expression CompileWhile(Ast<Token> ast)
        {
            Expression condition = ToBool(Convert(ast[0].Accept(this), typeof(iObject)));
            var body = Convert(ast[1].Accept(this), typeof(iObject));

            if(ast.Value.Type == kUNTIL || ast.Value.Type == kUNTIL_MOD)
            {
                condition = Not(condition);
            }

            var breakLabel = Label(typeof(iObject));
            var parm       = Parameter(typeof(iObject));
            breakExpressions.Push(Break(breakLabel, parm));

            try
            {
                if(ast[1].Value.Type == kBEGIN
                && (ast.Value.Type == kWHILE_MOD || ast.Value.Type == kUNTIL_MOD))
                {
                    // postfix while
                    // TODO is postfix if `begin' statement with no `rescue' or `ensure' clauses
                    throw new NotImplementedException();
                }

                throw new NotImplementedException();
            }
            finally
            {
                breakExpressions.Pop();
            }
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

        private Expression ToBool(Expression expr)
        {
            // (obj) => obj != null && !(obj is NilClass) && !(obj is FalseClass)

            var obj = Variable(typeof(iObject));

            return Block(
                new[] { obj },
                Assign(obj, expr),
                And(
                    NotEqual(obj, Constant(null)),
                    And(
                        Not(TypeIs(obj, typeof(NilClass))),
                        Not(TypeIs(obj, typeof(FalseClass)))
                    )
                )
            );
        }

        protected static readonly ConstructorInfo STRING_CTOR1  = Ctor<String>();
        protected static readonly ConstructorInfo STRING_CTOR2  = Ctor<String>(typeof(string));
        protected static readonly ConstructorInfo STRING_CTOR3  = Ctor<String>(typeof(String));
        protected static readonly ConstructorInfo SYMBOL_CTOR   = Ctor<Symbol>(typeof(string));

        protected static readonly Expression DFT_ELSE = Convert(Constant(new NilClass()), typeof(iObject));

        protected static ConstructorInfo Ctor<T>(params Type[] argTypes) => typeof(T).GetConstructor(argTypes);

        protected static MethodInfo Method<T>(string name, params Type[] argTypes) => typeof(T).GetMethod(name, argTypes);
    }
}
