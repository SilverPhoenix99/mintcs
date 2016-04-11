using Mint.Parse;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        public Compiler(string filename, Closure binding)
        {
            this.filename = filename;
            CurrentScope = new Scope(ScopeType.Method, binding);

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
            Register(kANDOP,          CompileAnd);
            Register(kOR,             CompileOr);
            Register(kOROP,           CompileOr);
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
            Register(kDOT2,           CompileRange);
            Register(kDOT3,           CompileRange);
            Register(kASSIGN,         CompileAssign);
            Register(kDOT,            CompileMethodInvoke);
            Register(kSELF,           CompileSelf);
            Register(tIDENTIFIER,     CompileIdentifier);
            Register(tOP_ASGN,        CompileOpAssign);
            Register(tWORDS_BEG,      CompileWords);
            Register(tQWORDS_BEG,     CompileWords);
            Register(tSYMBOLS_BEG,    CompileSymbolWords);
            Register(tQSYMBOLS_BEG,   CompileSymbolWords);
            Register(kLBRACE,         CompileHash);
            Register(tLABEL,          CompileLabel);
        }

        public Scope CurrentScope { get; protected set; }

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

            throw new ArgumentException($"Token type {ast.Value.Type} not registered.", nameof(ast));
        }

        protected virtual Expression CompileList(Ast<Token> ast)
        {
            return ast.List.Count == 1
                ? ast[0].Accept(this)
                : Block(typeof(iObject), ast.Select(_ => _.Accept(this)));
        }

        private static readonly Regex CLEAN_INTEGER = new Regex(@"[_BODX]", RegexOptions.Compiled);

        protected virtual Expression CompileInteger(Ast<Token> ast)
        {
            var tok = ast.Value;
            var str = CLEAN_INTEGER.Replace(tok.Value.ToUpper(), "");
            var numBase = (int) tok.Properties["num_base"];
            var val = System.Convert.ToInt64(str, numBase);
            return Constant(new Fixnum(val), typeof(iObject));
        }

        protected virtual Expression CompileFloat(Ast<Token> ast)
        {
            var tok = ast.Value;
            var str = tok.Value.Replace("_", "");
            var val = System.Convert.ToDouble(str, CultureInfo.InvariantCulture);
            return Constant(new Float(val), typeof(iObject));
        }

        protected virtual Expression CompileTrue(Ast<Token> ast = null)
        {
            return Constant(new TrueClass(), typeof(iObject));
        }

        protected virtual Expression CompileFalse(Ast<Token> ast = null)
        {
            return Constant(new FalseClass(), typeof(iObject));
        }

        protected virtual Expression CompileNil(Ast<Token> ast)
        {
            return CONSTANT_NIL;
        }

        protected virtual Expression CompileSymbol(Ast<Token> ast)
        {
            var content = ast.List[0];

            return content.IsList
                ? CompileSymbol(content.List)
                : Constant(new Symbol(content.Value.Value), typeof(iObject));
        }

        private Expression CompileSymbol(IEnumerable<Ast<Token>> content)
        {
            return Convert(
                New(
                    SYMBOL_CTOR,
                    Call(
                        Convert(
                            CompileString(New(STRING_CTOR1), content),
                            typeof(object)
                         ),
                        METHOD_OBJECT_TOSTRING,
                        null
                    )
                ),
                typeof(iObject)
            );
        }

        protected virtual Expression CompileString(Ast<Token> ast)
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

        protected virtual Expression CompileChar(Ast<Token> ast)
        {
            var first = New(STRING_CTOR3, CompileStringContent(ast));

            return ast.List.Count == 0
                ? Convert(first, typeof(iObject))
                : CompileString(first, ast);
        }

        private Expression CompileString(Expression first, IEnumerable<Ast<Token>> ast)
        {
            var list = ast.Select(_ => _.Accept(this))
                .Select(_ => _.Type == typeof(String)
                    ? _
                    : New(
                        STRING_CTOR2,
                        Call(
                            Convert(_, typeof(object)),
                            METHOD_OBJECT_TOSTRING,
                            null))
                    );

            list = new[] { first }.Concat(list);
            first = list.Aggregate((l, r) => Call(l, METHOD_STRING_CONCAT, r));

            return Convert(first, typeof(iObject));
        }

        protected virtual Expression CompileStringContent(Ast<Token> ast)
        {
            return Constant(new String(ast.Value.Value));
        }

        protected virtual Expression CompileUMinusNum(Ast<Token> ast)
        {
            return Negate(ast[0].Accept(this));
        }

        protected virtual Expression CompileIf(Ast<Token> ast)
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

        protected virtual Expression CompileQMark(Ast<Token> ast)
        {
            var condition = ToBool(ast[0].Accept(this));
            var ifTrue    = ast[1].Accept(this);
            var ifFalse   = ast[2].Accept(this);
            return Condition(condition, ifTrue, ifFalse, typeof(iObject));
        }

        protected virtual Expression CompileNot(Ast<Token> ast)
        {
            return Condition(
                ToBool(ast[0].Accept(this)),
                CompileFalse(),
                CompileTrue(),
                typeof(iObject)
            );
        }

        protected virtual Expression CompileAnd(Ast<Token> ast) => AndOperation(ast[0].Accept(this), ast[1].Accept(this));

        protected static Expression AndOperation(Expression left, Expression right)
        {
            if(left is ConstantExpression || left is ParameterExpression)
            {
                return Condition(ToBool(left), right, left, typeof(iObject));
            }

            var leftVar = Variable(typeof(iObject));

            return Block(
                typeof(iObject),
                new[] { leftVar },
                Assign(leftVar, left),
                Condition(ToBool(leftVar), right, leftVar, typeof(iObject))
            );
        }

        protected virtual Expression CompileOr(Ast<Token> ast) => OrOperation(ast[0].Accept(this), ast[1].Accept(this));

        protected static Expression OrOperation(Expression left, Expression right)
        {
            if(left is ConstantExpression || left is ParameterExpression)
            {
                return Condition(ToBool(left), left, right, typeof(iObject));
            }

            var leftVar = Variable(typeof(iObject));

            return Block(
                typeof(iObject),
                new[] { leftVar },
                Assign(leftVar, left),
                Condition(ToBool(leftVar), leftVar, right, typeof(iObject))
            );
        }

        protected virtual Expression CompileLineNumber(Ast<Token> ast)
        {
            return Constant(new Fixnum(ast.Value.Location.Item1), typeof(iObject));
        }

        protected virtual Expression CompileFileName(Ast<Token> ast)
        {
            return Constant(new String(filename), typeof(iObject));
        }

        protected virtual Expression CompileWhile(Ast<Token> ast) => WithScope(ast, ScopeType.While, CompileScopedWhile);

        protected virtual Expression CompileScopedWhile(Ast<Token> ast)
        {
            var breakLabel = CurrentScope.Label("break", typeof(iObject));
            var nextLabel = CurrentScope.Label("next");

            var condition = ToBool(ast[0].Accept(this));
            if(ast.Value.Type == kUNTIL || ast.Value.Type == kUNTIL_MOD)
            {
                condition = Negate(condition);
            }

            if(ast[1].Value?.Type == kBEGIN
            && (ast.Value.Type == kWHILE_MOD || ast.Value.Type == kUNTIL_MOD))
            {
                CurrentScope.Labels["redo"] = nextLabel;

                // postfix `while'
                // it's postfix if `begin' statement with no `rescue' or `ensure' clauses

                return Loop(
                    Block(
                        typeof(iObject),
                        ast[1].Accept(this),
                        IfThen(condition, Continue(nextLabel)),
                        Break(breakLabel, CONSTANT_NIL, typeof(iObject))
                    ),
                    breakLabel,
                    nextLabel
                );
            }

            var redoLabel = CurrentScope.Label("redo");

            return Loop(
                Condition(
                    condition,
                    Block(
                        typeof(iObject),
                        Label(redoLabel),
                        ast[1].Accept(this)
                    ),
                    Break(breakLabel, CONSTANT_NIL, typeof(iObject)),
                    typeof(iObject)
                ),
                breakLabel,
                nextLabel
            );
        }

        protected virtual Expression CompileBreak(Ast<Token> ast)
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

        protected virtual Expression CompileNext(Ast<Token> ast)
        {
            if(CurrentScope.Type == ScopeType.While)
            {
                // value is ignored, so there's no need to compile it
                return Continue(CurrentScope.Label("next"), typeof(iObject));
            }

            throw new NotImplementedException();
        }

        protected virtual Expression CompileRetry(Ast<Token> ast)
        {
            if(CurrentScope.Type == ScopeType.While)
            {
                // value is ignored, so there's no need to compile it
                throw new SyntaxError(filename, ast.Value.Location.Item1, "Invalid retry");
            }

            throw new NotImplementedException();
        }

        protected virtual Expression CompileRedo(Ast<Token> ast)
        {
            if(CurrentScope.Type == ScopeType.While)
            {
                // value is ignored, so there's no need to compile it
                return Goto(CurrentScope.Label("redo"), typeof(iObject));
            }

            throw new NotImplementedException();
        }

        protected virtual Expression CompileArray(Ast<Token> ast)
        {
            return Convert(
                ListInit(New(ARRAY_CTOR), ast.Select(_ => _.Accept(this))),
                typeof(iObject)
            );
        }

        protected virtual Expression CompileRange(Ast<Token> ast)
        {
            return Convert(
                New(
                    RANGE_CTOR,
                    ast[0].Accept(this),
                    ast[1].Accept(this),
                    Constant(ast.Value.Type == kDOT3)
                ),
                typeof(iObject)
            );
        }

        protected virtual Expression CompileAssign(Ast<Token> ast)
        {
            var lval = ast[0].Value;

            switch(lval.Type)
            {
                case kSELF:
                    throw new SyntaxError(filename, ast[0].Value.Location.Item1, "Can't change the value of self");

                case tIDENTIFIER:
                {
                    var name = new Symbol(ast[0].Value.Value);
                    var local = CurrentScope.Variable(name);
                    var rhs = ast[1].Accept(this);
                    return Assign(local, rhs);
                }

                case kDOT:
                {
                    var lhs = ast[0][0].Accept(this);
                    var name = new Symbol(ast[0][1].Value.Value + "=");
                    var rhs = ast[1].Accept(this);
                    return MakeCall(lhs, name, rhs);
                }

                case kLBRACK2:
                {
                    var lhs = ast[0][0].Accept(this);
                    var rhs = ast[0][1].Select(_ => _.Accept(this))
                        .Concat(new[] { ast[1].Accept(this) }).ToArray();
                    return MakeCall(lhs, Symbol.ASET, rhs);
                }

                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual Expression CompileMethodInvoke(Ast<Token> ast)
        {
            var methodName = new Symbol(ast[1].Value.Value);

            // TODO if empty => private method
            if(ast[0].IsList && ast[0].List.Count == 0)
            {
                throw new NotImplementedException("private method");
            }

            var instance = ast[0].Accept(this);
            var args = ast[2].Select(_ => _.Accept(this)).ToArray();
            return MakeCall(instance, methodName, args);
        }

        protected virtual Expression CompileSelf(Ast<Token> ast)
        {
            return Constant(CurrentScope.Closure.Self);
        }

        protected virtual Expression CompileIdentifier(Ast<Token> ast)
        {
            var name = new Symbol(ast.Value.Value);
            if(!CurrentScope.Closure.IsDefined(name))
            {
                throw new NotImplementedException("variable not found. methods not implemented.");
            }

            return CurrentScope.Variable(name);
        }

        protected virtual Expression CompileOpAssign(Ast<Token> ast)
        {
            var left = ast[0].Value.Type == tIDENTIFIER
                ? CurrentScope.Variable(new Symbol(ast[0].Value.Value))
                : ast[0].Accept(this);

            var right = ast[1].Accept(this);

            switch(ast.Value.Value)
            {
                case "||":
                    return OrOperation(left, Assign(left, right));

                case "&&":
                    return AndOperation(left, Assign(left, right));

                default:
                    throw new NotImplementedException($"unknown operation `{ast.Value.Value}='");
            }
        }

        protected virtual Expression CompileWords(Ast<Token> ast)
        {
            var lists = from list in GroupWords(ast)
                        where list.Count != 0
                        select CompileString(New(STRING_CTOR1), list);

            return Convert(
                ListInit(New(ARRAY_CTOR), lists),
                typeof(iObject)
            );
        }

        protected virtual Expression CompileSymbolWords(Ast<Token> ast)
        {
            var lists = from list in GroupWords(ast)
                        where list.Count != 0
                        select CompileSymbol(list);

            return Convert(
                ListInit(New(ARRAY_CTOR), lists),
                typeof(iObject)
            );
        }

        private static IEnumerable<List<Ast<Token>>> GroupWords(IEnumerable<Ast<Token>> list)
        {
            return list.Aggregate(
                new List<List<Ast<Token>>> {new List<Ast<Token>>()},
                (l, node) =>
                {
                    if(node.Value.Type == tSPACE)
                    {
                        l.Add(new List<Ast<Token>>());
                    }
                    else
                    {
                        l.Last().Add(node);
                    }
                    return l;
                });
        }

        protected virtual Expression CompileHash(Ast<Token> ast)
        {
            if(ast.List.Count == 0)
            {
                return Constant(new Hash(), typeof(iObject));
            }

            var list = new List<Expression>(ast.List.Count);
            var hash = Variable(typeof(Hash), "hash");
            list.Add(Assign(hash, New(HASH_CTOR)));

            foreach(var node in ast.List)
            {
                switch(node.Value.Type)
                {
                    case kASSOC:
                    {
                        var key   = node[0].Accept(this);
                        var value = node[1].Accept(this);
                        list.Add(HashAppend(hash, key, value));
                        break;
                    }

                    case tLABEL:
                    {
                        var key = node.Accept(this);
                        var value = node[0].Accept(this);
                        list.Add(HashAppend(hash, key, value));
                        break;
                    }

                    case tLABEL_END:
                    {
                        var key   = CompileSymbol(node[0].List);
                        var value = node[1].Accept(this);
                        list.Add(HashAppend(hash, key, value));
                        break;
                    }

                    case kDSTAR:
                    {
                        // TODO: { **h }
                        throw new NotImplementedException("**h");
                    }

                    default:
                        throw new NotImplementedException();
                }
            }

            //TODO : "warning: key {key.inspect} is duplicated and overwritten on line #{line}"

            list.Add(hash);

            return Block(
                typeof(iObject),
                new[] { hash },
                list
            );
        }

        protected virtual Expression CompileLabel(Ast<Token> ast)
        {
            var value = ast.Value.Value;
            return Constant(
                new Symbol(value.Substring(0, value.Length - 1)),
                typeof(iObject)
            );
        }

        private static Expression HashAppend(Expression hash, Expression key, Expression value) =>
            Assign(Property(hash, MEMBER_HASH_ITEM, key), value);

        private Expression WithScope(Ast<Token> ast, ScopeType type, Func<Ast<Token>, Expression> action)
        {
            CurrentScope = CurrentScope.Enter(type);
            try
            {
                return action(ast);
            }
            finally
            {
                CurrentScope = CurrentScope.Previous;
            }
        }

        protected static readonly ConstructorInfo STRING_CTOR1 = Reflector.Ctor<String>();
        protected static readonly ConstructorInfo STRING_CTOR2 = Reflector.Ctor<String>(typeof(string));
        protected static readonly ConstructorInfo STRING_CTOR3 = Reflector.Ctor<String>(typeof(String));
        protected static readonly ConstructorInfo SYMBOL_CTOR  = Reflector.Ctor<Symbol>(typeof(string));
        protected static readonly ConstructorInfo ARRAY_CTOR   = Reflector.Ctor<Array>();
        protected static readonly ConstructorInfo RANGE_CTOR   = Reflector.Ctor<Range>(typeof(iObject), typeof(iObject), typeof(bool));
        protected static readonly ConstructorInfo HASH_CTOR    = Reflector.Ctor<Hash>();

        protected static readonly MethodInfo METHOD_OBJECT_TOSTRING = Reflector<object>.Method(_ => _.ToString());
        protected static readonly MethodInfo METHOD_STRING_CONCAT   = Reflector<String>.Method(_ => _.Concat(null));
        protected static readonly PropertyInfo MEMBER_HASH_ITEM     = Reflector<Hash>.Property(_ => _[default(iObject)]);
        protected static readonly PropertyInfo MEMBER_CALLSITE_CALL = Reflector<MethodBinding.CallSite>.Property(_ => _.Call);

        protected static readonly Expression CONSTANT_NIL = Constant(new NilClass(), typeof(iObject));
        protected static readonly Expression EMPTY_ARRAY  = Constant(new iObject[0]);
        
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

            if(expr is ParameterExpression)
            {
                return And(
                    NotEqual(expr, Constant(null)),
                    And(
                        Not(TypeIs(expr, typeof(NilClass))),
                        Not(TypeIs(expr, typeof(FalseClass)))
                    )
                );
            }

            var parm = Variable(typeof(iObject));

            return Block(
                new[] { parm },
                Assign(parm, expr),
                And(
                    NotEqual(parm, Constant(null)),
                    And(
                        Not(TypeIs(parm, typeof(NilClass))),
                        Not(TypeIs(parm, typeof(FalseClass)))
                    )
                )
            );
        }

        protected static Expression Negate(Expression expr) =>
            expr.NodeType == ExpressionType.Not ? ((UnaryExpression) expr).Operand : Not(expr);

        private static Expression MakeCall(Expression instance, Symbol methodName, params Expression[] args)
        {
            var arity   = new Range(new Fixnum(0), new Fixnum(0));
            var site    = new MethodBinding.CallSite(methodName, arity);
            var call    = Property(Constant(site), MEMBER_CALLSITE_CALL);
            var argList = args.Length == 0 ? EMPTY_ARRAY : NewArrayInit(typeof(iObject), args);
            return Invoke(call, instance, argList);
        }
    }
}
