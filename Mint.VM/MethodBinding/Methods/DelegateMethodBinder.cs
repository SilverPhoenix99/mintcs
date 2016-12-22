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

            var method = Property(Constant(Lambda), nameof(Lambda.Method));
            var bindExpression = ArgumentBundle.Expressions.Bind(frame.Arguments, method);

            return Block(
                new[] { argumentsArray },
                Assign(argumentsArray, bindExpression),
                CreateBody(frame.Instance, argumentsArray)
            );
        }

        private Expression CreateBody(Expression instance, Expression argumentsArray)
        {
            var parameters = Lambda.Method.GetParameters();
            instance = instance.Cast(parameters[offset - 1].ParameterType);

            var convertedArgs = parameters.Skip(offset).Select(p => ConvertArgument(argumentsArray, p));
            var arguments = new[] { instance }.Concat(convertedArgs);
            
            return Box(Invoke(Constant(Lambda), arguments));
        }

        private Expression ConvertArgument(Expression argumentArray, ParameterInfo parameter)
        {
            var argument = ArrayIndex(argumentArray, Constant(parameter.Position - offset));
            return TryConvert(argument, parameter.ParameterType);
        }

        public static class Reflection
        {
            public static readonly ConstructorInfo Ctor =
                Reflector<DelegateMethodBinder>.Ctor<Symbol, Module, Delegate>();            
        }

        public static class Expressions
        {
            public static NewExpression New(Expression name, Expression owner, Expression lambda) =>
                Expression.New(Reflection.Ctor, name, owner, lambda);
        }
    }
}