using Microsoft.CSharp.RuntimeBinder;
using Mint.Parse;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static Mint.Parse.TokenType;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation
{
    public class Compiler : AstVisitor<Token, Expression>
    {
        private readonly Dictionary<TokenType, Func<Ast<Token>, Expression>> actions =
            new Dictionary<TokenType, Func<Ast<Token>, Expression>>();
        protected readonly string filename;

        protected readonly Stack<Scope> scopes = new Stack<Scope>();

        public Compiler(string filename)
        {
            this.filename = filename;
            scopes.Push(new Scope(ScopeType.Method));

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
            Register(kBREAK,          CompileBreak);
            Register(kNEXT,           CompileNext);
            Register(kRETRY,          CompileRetry);
            Register(kREDO,           CompileRedo);
            Register(kBEGIN,          CompileList);
            Register(kLBRACK,         CompileArray);
        }

        protected Scope CurrentScope => scopes.Peek();

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
            return ast.List.Count == 1
                ? ast[0].Accept(this)
                : Block(ast.List.Select(_ => _.Accept(this)));
        }

        protected Expression CompileInteger(Ast<Token> ast)
        {
            var tok = ast.Value;
            var str = CLEAN_INTEGER.Replace(tok.Value.ToUpper(), "");
            var num_base = (int) tok.Properties["num_base"];
            var val = System.Convert.ToInt64(str, num_base);
            return Constant(new Fixnum(val), typeof(iObject));
        }

        protected Expression CompileFloat(Ast<Token> ast)
        {
            var tok = ast.Value;
            var str = tok.Value.Replace("_", "");
            var val = System.Convert.ToDouble(str, CultureInfo.InvariantCulture);
            return Constant(new Float(val), typeof(iObject));
        }

        protected Expression CompileTrue(Ast<Token> ast = null)
        {
            return Constant(new TrueClass(), typeof(iObject));
        }

        protected Expression CompileFalse(Ast<Token> ast = null)
        {
            return Constant(new FalseClass(), typeof(iObject));
        }

        protected Expression CompileNil(Ast<Token> ast)
        {
            return CONSTANT_NIL;
        }

        protected Expression CompileSymbol(Ast<Token> ast)
        {
            var content = ast.List[0];

            if(!content.IsList)
            {
                return Constant(new Symbol(content.Value.Value), typeof(iObject));
            }
            
            return Convert(
                New(
                    SYMBOL_CTOR,
                    Call(CompileString(New(STRING_CTOR1), content), "ToString", null)
                ),
                typeof(iObject)
            );
        }

        protected Expression CompileString(Ast<Token> ast)
        {
            if(ast.List.Count == 1 && ast[0].Value.Type == tSTRING_CONTENT)
            {
                return Convert(
                    New(
                        STRING_CTOR3,
                        CompileStringContent(ast[0])
                    ),
                    typeof(iObject)
                );
            }
            
            return CompileString(New(STRING_CTOR1), ast);
        }

        protected Expression CompileChar(Ast<Token> ast)
        {
            var first = New(STRING_CTOR3, CompileStringContent(ast));

            return ast.List.Count == 0
                ? Convert(first, typeof(iObject))
                : CompileString(first, ast);
        }

        private Expression CompileString(Expression first, IEnumerable<Ast<Token>> ast)
        {
            var list = ast.Select(_ => _.Accept(this))
                .Select(_ => _.Type == typeof(String) ? _ : New(STRING_CTOR2, Call(_, "ToString", null)));

            list = new[] { first }.Concat(list);
            first = list.Aggregate((l, r) => Call(l, "Concat", null, r));

            return Convert(first, typeof(iObject));
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
            var condition = ToBool(ast[0].Accept(this));

            if(ast.Value.Type == kUNLESS || ast.Value.Type == kUNLESS_MOD)
            {
                condition = Negate(condition);
            }

            var ifTrue = ast[1].Accept(this);

            var ifFalse = ast.List.Count < 3 || ast[2].List.Count == 0
                        ? CONSTANT_NIL
                        : ast[2].Accept(this);

            return Condition(condition, ifTrue, ifFalse, typeof(iObject));
        }

        protected Expression CompileQMark(Ast<Token> ast)
        {
            var condition = ToBool(ast[0].Accept(this));
            var ifTrue    = ast[1].Accept(this);
            var ifFalse   = ast[2].Accept(this);
            return Condition(condition, ifTrue, ifFalse, typeof(iObject));
        }

        protected Expression CompileNot(Ast<Token> ast)
        {
            return Condition(
                ToBool(ast[0].Accept(this)),
                CompileFalse(),
                CompileTrue(),
                typeof(iObject)
            );
        }

        protected Expression CompileAnd(Ast<Token> ast)
        {
            var left  = ast[0].Accept(this);
            var right = ast[1].Accept(this);
            return Condition(ToBool(left), right, left, typeof(iObject));
        }

        protected Expression CompileOr(Ast<Token> ast)
        {
            var left  = ast[0].Accept(this);
            var right = ast[1].Accept(this);
            return Condition(ToBool(left), left, right, typeof(iObject));
        }

        protected Expression CompileLineNumber(Ast<Token> ast)
        {
            return Constant(new Fixnum(ast.Value.Location.Item1), typeof(iObject));
        }

        protected Expression CompileFileName(Ast<Token> ast)
        {
            return Constant(new String(filename), typeof(iObject));
        }

        protected Expression CompileWhile(Ast<Token> ast) => WithScope(ast, ScopeType.While, CompileWhile);

        protected Expression CompileWhile(Ast<Token> ast, Scope scope)
        {
            var breakLabel = scope.Label("break", typeof(iObject));
            var nextLabel = scope.Label("next");

            var body = ast[1].Accept(this);

            var condition = ToBool(ast[0].Accept(this));
            if(ast.Value.Type == kUNTIL || ast.Value.Type == kUNTIL_MOD)
            {
                condition = Negate(condition);
            }

            if(ast[1].Value?.Type == kBEGIN
            && (ast.Value.Type == kWHILE_MOD || ast.Value.Type == kUNTIL_MOD))
            {
                scope.Labels["redo"] = nextLabel;

                // postfix `while'
                // it's postfix if `begin' statement with no `rescue' or `ensure' clauses

                return Loop(
                    Block(
                        body,
                        IfThen(condition, Continue(nextLabel)),
                        Break(breakLabel, CONSTANT_NIL, typeof(iObject))
                    ),
                    breakLabel,
                    nextLabel
                );
            }

            var redoLabel = scope.Label("redo");

            return Loop(
                Condition(
                    condition,
                    Block(
                        Label(redoLabel),
                        body
                    ),
                    Break(breakLabel, CONSTANT_NIL, typeof(iObject)),
                    typeof(iObject)
                ),
                breakLabel,
                nextLabel
            );
        }

        protected Expression CompileBreak(Ast<Token> ast)
        {
            Expression value;
            switch(ast.List.Count)
            {
                case 0:
                    value = CONSTANT_NIL;
                    break;

                case 1:
                    if(ast.IsList)
                    {
                        throw new NotImplementedException();
                    }

                    value = ast[0].Accept(this);
                    break;

                default:
                    value = CompileArray(ast);
                    break;
            }

            return Break(CurrentScope.Label("break"), value, typeof(iObject));
        }

        protected Expression CompileNext(Ast<Token> ast)
        {
            if(CurrentScope.Type == ScopeType.While)
            {
                // value is ignored, so there's no need to compile it
                return Continue(CurrentScope.Label("next"), typeof(iObject));
            }

            throw new NotImplementedException();
        }

        protected Expression CompileRetry(Ast<Token> ast)
        {
            if(CurrentScope.Type == ScopeType.While)
            {
                // value is ignored, so there's no need to compile it
                throw new SyntaxError(filename, ast.Value.Location.Item1, "Invalid retry");
            }

            throw new NotImplementedException();
        }

        protected Expression CompileRedo(Ast<Token> ast)
        {
            if(CurrentScope.Type == ScopeType.While)
            {
                // value is ignored, so there's no need to compile it
                return Goto(CurrentScope.Label("redo"), typeof(iObject));
            }

            throw new NotImplementedException();
        }

        protected Expression CompileArray(Ast<Token> ast)
        {
            return Convert(
                New(
                    ARRAY_CTOR,
                    NewArrayInit(
                        typeof(iObject),
                        ast.Select(_ => _.Accept(this))
                    )
                ),
                typeof(iObject)
            );
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

        private Expression WithScope(Ast<Token> ast, ScopeType type, Func<Ast<Token>, Scope, Expression> action)
        {
            var scope = new Scope(type);
            scopes.Push(scope);

            try
            {
                return action(ast, scope);
            }
            finally
            {
                scopes.Pop();
            }
        }

        protected static readonly ConstructorInfo STRING_CTOR1 = Ctor<String>();
        protected static readonly ConstructorInfo STRING_CTOR2 = Ctor<String>(typeof(string));
        protected static readonly ConstructorInfo STRING_CTOR3 = Ctor<String>(typeof(String));
        protected static readonly ConstructorInfo SYMBOL_CTOR  = Ctor<Symbol>(typeof(string));
        protected static readonly ConstructorInfo ARRAY_CTOR   = Ctor<Array>(typeof(iObject[]));

        protected static readonly Expression CONSTANT_NIL = Constant(new NilClass(), typeof(iObject));

        protected static ConstructorInfo Ctor<T>(params Type[] argTypes) => typeof(T).GetConstructor(argTypes);

        protected static MethodInfo Method<T>(string name, params Type[] argTypes) => typeof(T).GetMethod(name, argTypes);

        private static Expression ToBool(Expression expr)
        {
            var cnst = expr as ConstantExpression;
            if(cnst != null)
            {
                return Constant(!(cnst.Value is NilClass) && !(cnst.Value is FalseClass));
            }

            // (obj) => obj != null && !(obj is NilClass) && !(obj is FalseClass)

            if(expr.Type != typeof(iObject))
            {
                expr = Convert(expr, typeof(iObject));
            }

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

        protected static Expression Negate(Expression expr) =>
            expr.NodeType == ExpressionType.Not ? ((UnaryExpression) expr).Operand : Not(expr);
    }
}
