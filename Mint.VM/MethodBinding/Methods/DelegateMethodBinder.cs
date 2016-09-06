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
        private Delegate Lambda { get; }

        public DelegateMethodBinder(Symbol name, Module owner, Delegate lambda/*, params Attribute[] parameterAttributes*/)
            : base(name, owner)
        {
            Lambda = lambda;

            //this.parameterAttributes = parameterAttributes;
            Arity = Lambda.Method.GetParameterCounter().Arity;
        }

        private DelegateMethodBinder(Symbol newName, DelegateMethodBinder other)
            : base(newName, other)
        {
            Lambda = other.Lambda;
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
            var bindExpression = ArgumentBundle.Expressions.Bind(frame.Arguments, Constant(Lambda.Method));
            Expression argumentCheck = NotEqual(argumentArray, Constant(null));

            if(Lambda.Method.GetParameters().Length > 1)
            {
                argumentCheck = AndAlso(argumentCheck, TypeCheckArgumentsExpression(argumentArray));
            }

            return Block(
                Assign(argumentArray, bindExpression),
                argumentCheck
            );
        }

        private Expression TypeCheckArgumentsExpression(Expression argumentArray) =>
            Enumerable.Range(1, Lambda.Method.GetParameters().Length - 1)
                .Select(i => TypeCheckArgumentExpression(argumentArray, i)).Aggregate(AndAlso);

        private Expression TypeCheckArgumentExpression(Expression argumentArray, int position)
        {
            var argument = ArrayIndex(argumentArray, Constant(position - 1));
            var parameter = Lambda.Method.GetParameters()[position];
            return TypeIs(argument, parameter.ParameterType);
        }

        private Expression CreateBody(CallFrameBinder frame, Expression argumentArray)
        {
            var instance = frame.Instance;

            var parameters = Lambda.Method.GetParameters();
            instance = instance.Cast(parameters[0].ParameterType);

            var convertedArgs = parameters.Skip(1).Select(p => ConvertArgument(argumentArray, p));
            var arguments = new[] { instance }.Concat(convertedArgs);
            
            return Box(Invoke(Constant(Lambda), arguments));
        }

        private static Expression ConvertArgument(Expression argumentArray, ParameterInfo parameter)
        {
            var argument = ArrayIndex(argumentArray, Constant(parameter.Position - 1));
            return TryConvert(argument, parameter.ParameterType);
        }

        private static Exception ThrowInvalidConversion()
        {
            // TODO

            return new TypeError("no implicit conversion exists");
        }
        
        public static class Reflection
        {
            public static readonly MethodInfo ThrowInvalidConversion = Reflector.Method(
                () => ThrowInvalidConversion()
            );
        }

        public static class Expressions
        {
            public static MethodCallExpression ThrowInvalidConversion() => Call(Reflection.ThrowInvalidConversion);
        }
    }
}