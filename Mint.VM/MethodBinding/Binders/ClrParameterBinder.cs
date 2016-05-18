using System.Linq.Expressions;
using Mint.Reflection;

namespace Mint.MethodBinding.Binders
{
    internal interface ClrParameterBinder
    {
        int ArgumentIndex { get; }
        CallInfo CallInfo { get; }
        MethodInformation MethodInformation { get; }
        Expression[] Arguments { get; }

        Expression Bind();
    }

    internal abstract class BaseParameterBinder : ClrParameterBinder
    {
        public int ArgumentIndex { get; }
        public CallInfo CallInfo { get; }
        public MethodInformation MethodInformation { get; }
        public Expression[] Arguments { get; }

        public BaseParameterBinder(
            int argumentIndex,
            CallInfo callInfo,
            MethodInformation methodInformation,
            Expression[] arguments
        )
        {
            ArgumentIndex = argumentIndex;
            CallInfo = callInfo;
            MethodInformation = methodInformation;
            Arguments = arguments;
        }

        public abstract Expression Bind();
    }

    internal class PrefixRequiredParameterBinder : BaseParameterBinder
    {
        public PrefixRequiredParameterBinder(
            int argumentIndex,
            CallInfo callInfo,
            MethodInformation methodInformation,
            Expression[] arguments
        )
            : base(argumentIndex, callInfo, methodInformation, arguments)
        { }

        public override Expression Bind() => Arguments[ArgumentIndex];
    }

    internal class OptionalParameterBinder : BaseParameterBinder
    {
        public OptionalParameterBinder(
            int argumentIndex,
            CallInfo callInfo,
            MethodInformation methodInformation,
            Expression[] arguments
        )
            : base(argumentIndex, callInfo, methodInformation, arguments)
        { }

        public override Expression Bind()
        {
            if(ArgumentIndex < Arguments.Length)
            {
                return Arguments[ArgumentIndex];
            }

            var parameter = MethodInformation.MethodInfo.GetParameters()[ArgumentIndex];
            var defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;
            return Expression.Constant(defaultValue);
        }
    }
}