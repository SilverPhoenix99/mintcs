using Mint.Binding;
using Mint.Binding.Arguments;
using Mint.Compilation.Components;
using Mint.Parse;
using Mint.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static Mint.Parse.TokenType;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation
{
    public sealed class Compiler : AstVisitor<Token, Expression>
    {
        private class Argument : Tuple<ArgumentKind, Expression>
        {
            public Argument(ArgumentKind kind, Expression arg) : base(kind, arg)
            { }

            public ArgumentKind Kind => Item1;
            public Expression Arg => Item2;
        }

        private static readonly ConstructorInfo STRING_CTOR = Reflector.Ctor<String>(typeof(string));
        private static readonly ConstructorInfo ARRAY_CTOR = Reflector.Ctor<Array>(typeof(IEnumerable<iObject>));
        private static readonly ConstructorInfo RANGE_CTOR = Reflector.Ctor<Range>(typeof(iObject), typeof(iObject), typeof(bool));
        private static readonly ConstructorInfo HASH_CTOR = Reflector.Ctor<Hash>();
        private static readonly MethodInfo METHOD_OBJECT_TOSTRING = Reflector<object>.Method(_ => _.ToString());
        private static readonly PropertyInfo MEMBER_HASH_ITEM = Reflector<Hash>.Property(_ => _[default(iObject)]);
        private static readonly PropertyInfo MEMBER_CALLSITE_CALL = Reflector<CallSite>.Property(_ => _.Call);

        private static readonly Expression EMPTY_ARRAY = Constant(new iObject[0]);

        internal static readonly Expression FALSE = Constant(new FalseClass(), typeof(iObject));
        internal static readonly Expression NIL = Constant(new NilClass(), typeof(iObject));
        internal static readonly Expression TRUE = Constant(new TrueClass(), typeof(iObject));

        private readonly Dictionary<TokenType, Func<Ast<Token>, Expression>> actions =
            new Dictionary<TokenType, Func<Ast<Token>, Expression>>();

        private readonly IDictionary<TokenType, CompilerComponent> components =
            new Dictionary<TokenType, CompilerComponent>();

        private string Filename { get; }

        public Scope CurrentScope { get; private set; }

        public CompilerComponent ListAction { get; set; }

        public Compiler(string filename, Closure binding)
        {
            Filename = filename;
            CurrentScope = new Scope(ScopeType.Method, binding);
            ListAction = new ListCompiler(this);

            Register(ListAction, kBEGIN, kELSE, tSTRING_DBEG);
            Register(new IntegerCompiler(this), tINTEGER);
            Register(new FloatCompiler(this), tFLOAT);
            Register(new ConstantCompiler(this), kFALSE, kNIL, kTRUE);
            Register(new SymbolCompiler(this), tSYMBEG);
            Register(new StringCompiler(this), tSTRING_BEG);
            Register(new CharCompiler(this), tCHAR);
            Register(new StringContentCompiler(this), tSTRING_CONTENT);
            Register(new WordsCompiler(this), tWORDS_BEG, tQWORDS_BEG);
            Register(new SymbolWordsCompiler(this), tSYMBOLS_BEG, tQSYMBOLS_BEG);

            Register(kUMINUS_NUM,     CompileUMinusNum);
            Register(kIF,             CompileIf);
            Register(kIF_MOD,         CompileIf);
            Register(kELSIF,          CompileIf);
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
            Register(kLBRACK,         CompileArray);
            Register(kDOT2,           CompileRange);
            Register(kDOT3,           CompileRange);
            Register(kASSIGN,         CompileAssign);
            Register(kDOT,            CompileMethodInvoke);
            Register(kSELF,           CompileSelf);
            Register(tIDENTIFIER,     CompileIdentifier);
            Register(tOP_ASGN,        CompileOpAssign);
            Register(kLBRACE,         CompileHash);
            Register(tLABEL,          CompileLabel);
            Register(kNOTOP,          CompileNotOperator);
            Register(kEQ,             CompileEqual);
            Register(kNEQ,            CompileNotEqual);
        }

        public void Register(TokenType type, Func<Ast<Token>, Expression> action)
        {
            actions[type] = action;
        }

        public void Register(CompilerComponent action, TokenType token)
        {
            components[token] = action;
        }

        public void Register(CompilerComponent action, params TokenType[] tokens)
        {
            foreach(var token in tokens)
            {
                Register(action, token);
            }
        }

        public Expression Visit(Ast<Token> ast)
        {
            if(ast.IsList)
            {
                return ListAction.Compile(ast);
            }

            Func<Ast<Token>, Expression> action;
            if(actions.TryGetValue(ast.Value.Type, out action))
            {
                return action(ast);
            }

            CompilerComponent component;
            if(components.TryGetValue(ast.Value.Type, out component))
            {
                return component.Compile(ast);
            }

            throw new ArgumentException($"Token type {ast.Value.Type} not registered.", nameof(ast));
        }

        internal static Expression NewString(Expression argument)
        {
            var call = Call(Convert(argument, typeof(object)), METHOD_OBJECT_TOSTRING, null);
            return New(STRING_CTOR, call);
        }
        
        private Expression CompileUMinusNum(Ast<Token> ast)
        {
            var number = (iObject) ((ConstantExpression) ast[0].Accept(this)).Value;

            if(number is Fixnum)
            {
                number = new Fixnum(-(long) (Fixnum) number);
            }
            else if(number is Float)
            {
                number = new Float(-(double) (Float) number);
            }
            else if(number is Complex)
            {
                throw new NotImplementedException();
                //number = ((Complex) number).Conjugate();
            }
            else
            {
                throw new NotImplementedException();
                //number = -(Rational) number;
            }

            return Constant(number, typeof(iObject));
        }

        private Expression CompileIf(Ast<Token> ast)
        {
            var condition = ToBool(ast[0].Accept(this));

            if(ast.Value.Type == kUNLESS || ast.Value.Type == kUNLESS_MOD)
            {
                condition = Negate(condition);
            }

            var ifTrue = ast[1].Accept(this);
            var ifFalse = ast.List.Count < 3 || ast[2].List.Count == 0 ? NIL : ast[2].Accept(this);

            return Condition(condition, ifTrue, ifFalse, typeof(iObject));
        }

        private Expression CompileQMark(Ast<Token> ast)
        {
            var condition = ToBool(ast[0].Accept(this));
            var ifTrue    = ast[1].Accept(this);
            var ifFalse   = ast[2].Accept(this);
            return Condition(condition, ifTrue, ifFalse, typeof(iObject));
        }

        private Expression CompileNot(Ast<Token> ast)
        {
            return Condition(ToBool(ast[0].Accept(this)), FALSE, TRUE, typeof(iObject));
        }

        private Expression CompileAnd(Ast<Token> ast) => AndOperation(ast[0].Accept(this), ast[1].Accept(this));

        private static Expression AndOperation(Expression left, Expression right)
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

        private Expression CompileOr(Ast<Token> ast) => OrOperation(ast[0].Accept(this), ast[1].Accept(this));

        private static Expression OrOperation(Expression left, Expression right)
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

        private Expression CompileLineNumber(Ast<Token> ast)
        {
            return Constant(new Fixnum(ast.Value.Location.Item1), typeof(iObject));
        }

        private Expression CompileFileName(Ast<Token> ast)
        {
            return Constant(new String(Filename), typeof(iObject));
        }

        private Expression CompileWhile(Ast<Token> ast) => WithScope(ast, ScopeType.While, CompileScopedWhile);

        private Expression CompileScopedWhile(Ast<Token> ast)
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
                        Break(breakLabel, NIL, typeof(iObject))
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
                    Break(breakLabel, NIL, typeof(iObject)),
                    typeof(iObject)
                ),
                breakLabel,
                nextLabel
            );
        }

        private Expression CompileBreak(Ast<Token> ast)
        {
            Expression value;
            switch(ast.List.Count)
            {
                case 0:
                    value = NIL;
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

        private Expression CompileNext(Ast<Token> ast)
        {
            if(CurrentScope.Type == ScopeType.While)
            {
                // value is ignored, so there's no need to compile it
                return Continue(CurrentScope.Label("next"), typeof(iObject));
            }

            throw new NotImplementedException();
        }

        private Expression CompileRetry(Ast<Token> ast)
        {
            if(CurrentScope.Type == ScopeType.While)
            {
                // value is ignored, so there's no need to compile it
                throw new SyntaxError(Filename, ast.Value.Location.Item1, "Invalid retry");
            }

            throw new NotImplementedException();
        }

        private Expression CompileRedo(Ast<Token> ast)
        {
            if(CurrentScope.Type == ScopeType.While)
            {
                // value is ignored, so there's no need to compile it
                return Goto(CurrentScope.Label("redo"), typeof(iObject));
            }

            throw new NotImplementedException();
        }

        private Expression CompileArray(Ast<Token> ast)
        {
            return CreateArray(ast.Select(_ => _.Accept(this)).ToArray());
        }

        internal static Expression CreateArray(params Expression[] values)
        {
            var array = New(ARRAY_CTOR, Constant(null, typeof(IEnumerable<iObject>)));
            return values.Length == 0
                ? (Expression) array
                : Convert(ListInit(array, values), typeof(iObject));
        }

        private Expression CompileRange(Ast<Token> ast)
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

        private Expression CompileAssign(Ast<Token> ast)
        {
            var lval = ast[0].Value;

            switch(lval.Type)
            {
                case kSELF:
                    throw new SyntaxError(Filename, ast[0].Value.Location.Item1, "Can't change the value of self");

                case tIDENTIFIER:
                {
                    var name = new Symbol(ast[0].Value.Value);
                    var local = CurrentScope.Variable(name);
                    var rhs = ast[1].Accept(this);
                    return Assign(local, rhs);
                }

                case kDOT:
                {
                    // <lhs>.<name>=(<rhs>)
                    var lhs   = ast[0][0].Accept(this);
                    var name  = new Symbol(ast[0][1].Value.Value + "=");
                    var rhs   = ast[1].Accept(this);
                    var value = Variable(typeof(iObject), "value");
                    var arg   = new Argument(ArgumentKind.Simple, value);

                    return Block(
                        typeof(iObject),
                        new[] { value },
                        Assign(value, rhs),
                        CreateInvoke(Visibility.Public, lhs, name, arg),
                        value
                    );
                }

                case kLBRACK2:
                {
                    // <lhs>[<args>+] = <rhs>  =>  <lhs>.[]=(<args>+, <rhs>)
                    var lhs   = ast[0][0].Accept(this);
                    var rhs   = ast[1].Accept(this);
                    var value = Variable(typeof(iObject), "value");
                    var arg   = new Argument(ArgumentKind.Simple, value);
                    var args  = ast[0][1].Select(CompileParameter).Concat(new[] { arg }).ToArray();

                    return Block(
                        typeof(iObject),
                        new[] { value },
                        Assign(value, rhs),
                        CreateInvoke(Visibility.Public, lhs, Symbol.ASET, args),
                        value
                    );
                }

                default:
                    throw new NotImplementedException();
            }
        }

        private Expression CompileMethodInvoke(Ast<Token> ast)
        {
            var methodName = new Symbol(ast[1].Value.Value);

            Expression instance;
            var lhs = ast[0];
            var visibility = lhs.Value?.Type == kSELF ? Visibility.Protected : Visibility.Public;
            if(lhs.IsList && lhs.List.Count == 0)
            {
                instance = CompileSelf();
                visibility = Visibility.Private;
            }
            else
            {
                instance = lhs.Accept(this);
            }

            var args = ast[2].Select(CompileParameter).ToArray();
            return CreateInvoke(visibility, instance, methodName, args);
        }

        private Argument CompileParameter(Ast<Token> ast)
        {
            switch(ast.Value.Type)
            {
                case tLABEL:
                {
                    var label = ast.Accept(this);
                    var value = ast[0].Accept(this);
                    return new Argument(ArgumentKind.Key, CreateArray(label, value));
                }

                case tLABEL_END: goto case kASSOC;
                case kASSOC:
                {
                    var label = ast[0].Accept(this);
                    var value = ast[1].Accept(this);
                    return new Argument(ArgumentKind.Key, CreateArray(label, value));
                }

                case kSTAR:
                    return new Argument(ArgumentKind.Rest, ast[0].Accept(this));

                case kDSTAR:
                    return new Argument(ArgumentKind.KeyRest, ast[0].Accept(this));

                case kAMPER:
                    return new Argument(ArgumentKind.Block, ast[0].Accept(this));

                default:
                    return new Argument(ArgumentKind.Simple, ast.Accept(this));
            }
        }

        private Expression CompileSelf(Ast<Token> ast = null)
        {
            return Constant(CurrentScope.Closure.Self);
        }

        private Expression CompileIdentifier(Ast<Token> ast)
        {
            var name = new Symbol(ast.Value.Value);
            if(!CurrentScope.Closure.IsDefined(name))
            {
                throw new NotImplementedException("variable not found. methods not implemented.");
            }

            return CurrentScope.Variable(name);
        }

        private Expression CompileOpAssign(Ast<Token> ast)
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

        private Expression CompileHash(Ast<Token> ast)
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

                    /*case tLABEL_END:
                    {
                        var key   = CompileSymbol(node[0].List);
                        var value = node[1].Accept(this);
                        list.Add(HashAppend(hash, key, value));
                        break;
                    }*/

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

        private Expression CompileLabel(Ast<Token> ast)
        {
            var value = ast.Value.Value;
            return Constant(
                new Symbol(value.Substring(0, value.Length - 1)),
                typeof(iObject)
            );
        }

        private Expression CompileNotOperator(Ast<Token> ast)
        {
            // TODO if protected in instance_eval, and lhs != self but same class => public

            var visibility = ast[0].Value?.Type == kSELF ? Visibility.Protected : Visibility.Public;
            return CreateInvoke(visibility, ast[0].Accept(this), Symbol.NOT_OP);
        }

        private Expression CompileEqual(Ast<Token> ast)
        {
            // TODO if protected in instance_eval, and lhs != self but same class => public

            var visibility = ast[0].Value?.Type == kSELF ? Visibility.Protected : Visibility.Public;
            var arg = new Argument(ArgumentKind.Simple, ast[1].Accept(this));
            return CreateInvoke(visibility, ast[0].Accept(this), Symbol.EQ, arg);
        }

        private Expression CompileNotEqual(Ast<Token> ast)
        {
            // TODO if protected in instance_eval, and lhs != self but same class => public

            var visibility = ast[0].Value?.Type == kSELF ? Visibility.Protected : Visibility.Public;
            var arg = new Argument(ArgumentKind.Simple, ast[1].Accept(this));
            return CreateInvoke(visibility, ast[0].Accept(this), Symbol.NEQ, arg);
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

        private static Expression Negate(Expression expr) =>
            expr.NodeType == ExpressionType.Not ? ((UnaryExpression) expr).Operand : Not(expr);

        private static Expression CreateInvoke(
            Visibility visibility,
            Expression instance,
            Symbol methodName,
            params Argument[] arguments
        )
        {
            var site = CallSite.Create(methodName, visibility, arguments.Select(_ => _.Kind));
            var call = Property(Constant(site), MEMBER_CALLSITE_CALL);
            var argList = arguments.Length == 0
                        ? EMPTY_ARRAY
                        : NewArrayInit(typeof(iObject), arguments.Select(_ => _.Arg));
            return Invoke(call, instance, argList);
        }

        internal static readonly MethodInfo DEBUGVIEW_INFO =
            typeof(Expression).GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic).GetMethod;

        internal static void DumpExpression(Expression expr)
        {
            Console.WriteLine(DEBUGVIEW_INFO.Invoke(expr, new object[0]));
            Console.WriteLine();
        }
    }
}
