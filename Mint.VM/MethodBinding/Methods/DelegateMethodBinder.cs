using Mint.Reflection;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.MethodBinding.Arguments;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Methods
{
    public sealed class DelegateMethodBinder : BaseMethodBinder
    {
        private readonly int offset;

        private Delegate Lambda { get; }

        public DelegateMethodBinder(Symbol name, Module owner, Delegate lambda)
            : base(name, owner)
        {
            Lambda = lambda;
            offset = lambda.Method.GetParameters()
                .First(_ => typeof(iObject).IsAssignableFrom(_.ParameterType)).Position + 1;
            Arity = Lambda.Method.GetParameterCounter().Arity;
        }

        private DelegateMethodBinder(Symbol newName, DelegateMethodBinder other)
            : base(newName, other)
        {
            Lambda = other.Lambda;
            offset = other.offset;
        }

        public override MethodBinder Duplicate(Symbol newName) => new DelegateMethodBinder(newName, this);

        public override Expression Bind(CallFrameBinder frame)
        {
            var argumentsArray = Variable(typeof(iObject[]), "arguments");

            var condition = CreateCondition(frame, argumentsArray);
            var body = CreateBody(frame, argumentsArray);
            var elseBody = Throw(Expressions.ThrowInvalidConversion(), typeof(iObject));

            return Block(
                new[] { argumentsArray },
                Condition(condition, body, elseBody, typeof(iObject))
            );
        }

        private Expression CreateCondition(CallFrameBinder frame, Expression argumentArray)
        {
            var method = Property(Constant(Lambda), nameof(Lambda.Method));
            var bindExpression = ArgumentBundle.Expressions.Bind(frame.Arguments, method);
            Expression argumentCheck = NotEqual(argumentArray, Constant(null));

            if(Lambda.Method.GetParameters().Length > offset)
            {
                argumentCheck = AndAlso(argumentCheck, TypeCheckArgumentsExpression(argumentArray));
            }

            return Block(
                Assign(argumentArray, bindExpression),
                argumentCheck
            );
        }

        private Expression TypeCheckArgumentsExpression(Expression argumentArray) =>
            Enumerable.Range(offset, Lambda.Method.GetParameters().Length - offset)
                .Select(i => TypeCheckArgumentExpression(argumentArray, i)).Aggregate(AndAlso);

        private Expression TypeCheckArgumentExpression(Expression argumentArray, int position)
        {
            var argument = ArrayIndex(argumentArray, Constant(position - offset));
            var parameter = Lambda.Method.GetParameters()[position];
            return TypeIs(argument, parameter.ParameterType);
        }

        private Expression CreateBody(CallFrameBinder frame, Expression argumentArray)
        {
            var instance = frame.Instance;

            var parameters = Lambda.Method.GetParameters();
            instance = instance.Cast(parameters[offset - 1].ParameterType);

            var convertedArgs = parameters.Skip(offset).Select(p => ConvertArgument(argumentArray, p));
            var arguments = new[] { instance }.Concat(convertedArgs);
            
            return Box(Invoke(Constant(Lambda), arguments));
        }

        private Expression ConvertArgument(Expression argumentArray, ParameterInfo parameter)
        {
            var argument = ArrayIndex(argumentArray, Constant(parameter.Position - offset));
            return TryConvert(argument, parameter.ParameterType);
        }

        private static Exception ThrowInvalidConversion()
        {
            // TODO

            return new TypeError("no implicit conversion exists");
        }

        public static class Reflection
        {
            public static readonly ConstructorInfo Ctor =
                Reflector<DelegateMethodBinder>.Ctor<Symbol, Module, Delegate>();

            public static readonly MethodInfo ThrowInvalidConversion = Reflector.Method(
                () => ThrowInvalidConversion()
            );
        }

        public static class Expressions
        {
            public static NewExpression New(Expression name, Expression owner, Expression lambda) =>
                Expression.New(Reflection.Ctor, name, owner, lambda);

            public static MethodCallExpression ThrowInvalidConversion() => Call(Reflection.ThrowInvalidConversion);
        }
    }
}