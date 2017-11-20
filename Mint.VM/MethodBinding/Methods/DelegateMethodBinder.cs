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
     * {
     *     CallFrame frame = CallFrame.Current;
     *     iObject[] arguments = frame.Arguments.Bind(@lambda.Method);
     *     // TODO: add arguments as local variables
     *     return Object.Box(@lambda.Invoke((<cast>) frame.Instance, (<cast>) arguments[0], ...));
     * }
     */
    public sealed class DelegateMethodBinder : BaseMethodBinder
    {
        private DelegateMetadata Lambda { get; }

        public DelegateMethodBinder(Symbol name, Module owner, DelegateMetadata lambda)
            : base(name, owner)
        {
            Lambda = lambda;
        }

        private DelegateMethodBinder(Symbol newName, DelegateMethodBinder other)
            : base(newName, other)
        {
            Lambda = other.Lambda;
        }
        
        public override MethodBinder Duplicate(Symbol newName) => new DelegateMethodBinder(newName, this);

        protected override Expression Bind()
        {
            var frameBinder = new CallFrameBinder();
            
            var method = Property(Constant(Lambda), nameof(Lambda.Method));
            var bundle = CallFrame.Expressions.Arguments(frameBinder.CallFrame);

            var instance = CallFrame.Expressions.Instance(frameBinder.CallFrame).Cast(Lambda.InstanceType);
            var convertedArgs = from p in Lambda.Method.Parameters
                                select ConvertArgument(frameBinder.Arguments, p);
            var arguments = new[] { instance }.Concat(convertedArgs);

            return Block(
                new[] { frameBinder.CallFrame, frameBinder.Arguments },
                Assign(frameBinder.CallFrame, CallFrame.Expressions.Current()),
                Assign(frameBinder.Arguments, ArgumentBundle.Expressions.Bind(bundle, method)),
                Box(Invoke(Constant(Lambda.Lambda), arguments))
            );
        }
        
        private static Expression ConvertArgument(Expression argumentArray, ParameterMetadata parameter)
        {
            var argument = ArrayIndex(argumentArray, Constant(parameter.Position));
            return TryConvert(argument, parameter.Parameter.ParameterType);
        }

        public static class Reflection
        {
            public static readonly ConstructorInfo Ctor =
                Reflector<DelegateMethodBinder>.Ctor<Symbol, Module, DelegateMetadata>();
        }
        
        public static class Expressions
        {
            public static NewExpression New(Expression name, Expression owner, Expression caller, Expression lambda)
                => Expression.New(Reflection.Ctor, name, owner, caller, lambda);
        }
    }
}