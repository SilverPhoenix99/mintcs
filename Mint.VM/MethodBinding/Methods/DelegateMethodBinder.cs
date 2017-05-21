using Mint.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.MethodBinding.Arguments;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Methods
{
    /*
     * Generated Stub:
     *
     * global iObject $instance;
     * global ArgumentBundle $bundle;
     *
     * {
     *     var arguments = $bundle.Bind(@methodInfo);
     *     return Object.Box(<Lambda>.Invoke((<cast>) $instance, (<cast>) arguments[0], ...));
     * }
     */
    public sealed class DelegateMethodBinder : BaseMethodBinder
    {
        private DelegateMetadata Lambda { get; }

        public DelegateMethodBinder(Symbol name, Module definer, DelegateMetadata lambda)
            : base(name, definer)
        {
            Lambda = lambda;
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
            instance = instance.Cast(Lambda.InstanceType);
            var convertedArgs = Lambda.Method.Parameters.Select(p => ConvertArgument(argumentsArray, p));
            var arguments = new[] { instance }.Concat(convertedArgs);
            
            return Box(Invoke(Constant(Lambda.Lambda), arguments));
        }

        private Expression ConvertArgument(Expression argumentArray, ParameterMetadata parameter)
        {
            var argument = GetArgument(argumentArray, parameter);
            return TryConvert(argument, parameter.Parameter.ParameterType);
        }

        private BinaryExpression GetArgument(Expression argumentArray, ParameterMetadata parameter) =>
            ArrayIndex(argumentArray, Constant(parameter.Position));

        public static class Reflection
        {
            public static readonly ConstructorInfo Ctor =
                Reflector<DelegateMethodBinder>.Ctor<Symbol, Module, DelegateMetadata>();
        }

        public static class Expressions
        {
            public static NewExpression New(Expression name,
                                            Expression definer,
                                            Expression caller,
                                            Expression lambda) =>
                Expression.New(Reflection.Ctor, name, definer, caller, lambda);
        }
    }
}