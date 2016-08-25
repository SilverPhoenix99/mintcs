using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.MethodBinding;
using Mint.MethodBinding.Arguments;
using Mint.Parse;
using Mint.Reflection;
using static System.Linq.Expressions.Expression;
using static Mint.MethodBinding.Visibility;
using static Mint.Parse.TokenType;

namespace Mint.Compilation
{
    internal static class CompilerUtils
    {
        private static readonly ConstructorInfo STRING_CTOR1 = Reflector.Ctor<String>();
        private static readonly ConstructorInfo STRING_CTOR2 = Reflector.Ctor<String>(typeof(String));
        private static readonly ConstructorInfo STRING_CTOR3 = Reflector.Ctor<String>(typeof(string));
        internal static readonly ConstructorInfo SYMBOL_CTOR = Reflector.Ctor<Symbol>(typeof(string));
        private static readonly MethodInfo STRING_CONCAT = Reflector<String>.Method(_ => _.Concat(default(string)));
        private static readonly MethodInfo OBJECT_TOSTRING = Reflector<object>.Method(_ => _.ToString());
        internal static readonly MethodInfo IS_NIL = Reflector.Method(() => NilClass.IsNil(default(object)));

        private static readonly MethodInfo CONVERT_TO_STRING =
            Reflector.Method(() => System.Convert.ToString(default(object)));

        private static readonly ConstructorInfo ARRAY_CTOR = Reflector.Ctor<Array>(typeof(IEnumerable<iObject>));
        internal static readonly ConstructorInfo RANGE_CTOR =
            Reflector.Ctor<Range>(typeof(iObject), typeof(iObject), typeof(bool));
        private static readonly ConstructorInfo HASH_CTOR = Reflector.Ctor<Hash>();
        private static readonly PropertyInfo MEMBER_HASH_ITEM = Reflector<Hash>.Property(_ => _[default(iObject)]);
        private static readonly PropertyInfo MEMBER_CALLSITE_CALL = Reflector<CallSite>.Property(_ => _.Call);

        internal static readonly MethodInfo INSTANCE_VARIABLE_GET = Reflector<iObject>.Method(
            _ => _.InstanceVariableGet(default(Symbol))
        );

        internal static readonly MethodInfo INSTANCE_VARIABLE_SET = Reflector<iObject>.Method(
            _ => _.InstanceVariableSet(default(Symbol), default(iObject))
        );

        internal static readonly Expression NIL = Constant(new NilClass(), typeof(iObject));
        internal static readonly Expression FALSE = Constant(new FalseClass(), typeof(iObject));
        internal static readonly Expression TRUE = Constant(new TrueClass(), typeof(iObject));
        private static readonly Expression EMPTY_ARRAY = Constant(System.Array.Empty<iObject>());

        public static Expression ToBool(Expression expr)
        {
            var cnst = expr as ConstantExpression;
            if(cnst != null)
            {
                return Constant(!(cnst.Value is NilClass) && !(cnst.Value is FalseClass));
            }

            // (obj) => obj != null && !(obj is NilClass) && !(obj is FalseClass)

            if(expr.Type != typeof(iObject))
            {
                expr = expr.Cast<iObject>();
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

        public static Expression Negate(Expression expr) =>
            expr.NodeType == ExpressionType.Not ? ((UnaryExpression) expr).Operand : Not(expr);

        public static Expression NewString(Expression argument = null)
        {
            if(argument == null)
            {
                return New(STRING_CTOR1);
            }

            argument = argument.StripConversions();

            if(argument.Type == typeof(String))
            {
                return New(STRING_CTOR2, argument);
            }

            if(argument.Type != typeof(string))
            {
                argument = argument.Cast<object>();
                argument = Expression.Call(argument, OBJECT_TOSTRING, null);
            }

            return New(STRING_CTOR3, argument);
        }

        public static Expression NewArray(params Expression[] values)
        {
            var array = New(ARRAY_CTOR, Constant(null, typeof(IEnumerable<iObject>)));
            return values.Length == 0
                ? array
                : ListInit(array, values).Cast<iObject>();
        }

        public static Expression NewHash() => New(HASH_CTOR).Cast<iObject>();

        public static Expression Call(
            Expression instance,
            Symbol methodName,
            Visibility visibility,
            params InvocationArgument[] arguments
        )
        {
            var site = CallSite.Create(methodName, visibility, arguments.Select(_ => _.Kind));
            var call = Constant(site).Property(MEMBER_CALLSITE_CALL);
            var argList = arguments.Length == 0
                        ? EMPTY_ARRAY
                        : NewArrayInit(typeof(iObject), arguments.Select(_ => _.Expression));
            return Invoke(call, instance, argList);
        }

        public static Expression StringConcat(Expression first, IEnumerable<Expression> contents)
        {
            contents = contents.Select(ExpressionExtensions.StripConversions);
            contents = new[] { first }.Concat(contents);
            first = contents.Aggregate(StringConcat);

            return first.Cast<iObject>();
        }

        private static Expression StringConcat(Expression left, Expression right)
        {
            right = Expression.Call(CONVERT_TO_STRING, right);
            return Expression.Call(left, STRING_CONCAT, right);
        }

        public static Visibility GetVisibility(Ast<Token> left)
        {
            // TODO if protected in instance_eval, and lhs != self but same class => public

            return left.Value?.Type == kSELF ? Protected : Public;
        }

        public static ArgumentKind GetArgumentKind(TokenType type)
        {
            switch(type)
            {
                case kSTAR:
                return ArgumentKind.Rest;

                case tLABEL_END: goto case kASSOC;
                case tLABEL: goto case kASSOC;
                case kASSOC:
                return ArgumentKind.Key;

                case kDSTAR:
                return ArgumentKind.KeyRest;

                case kDO: goto case kAMPER;
                case kLBRACE2: goto case kAMPER;
                case kAMPER:
                return ArgumentKind.Block;

                default:
                return ArgumentKind.Simple;
            }
        }
    }
}
