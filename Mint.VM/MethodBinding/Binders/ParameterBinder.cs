using Mint.Reflection;

namespace Mint.MethodBinding.Binders
{
    public abstract class ParameterBinder
    {
        public int ParameterIndex { get; }
        public CallInfo CallInfo { get; }
        public MethodInformation MethodInformation { get; }
        public ArgumentBundle Arguments { get; }

        public ParameterBinder(int parameterIndex, CallInfo callInfo, MethodInformation methodInformation)
        {
            ParameterIndex = parameterIndex;
            CallInfo = callInfo;
            MethodInformation = methodInformation;
        }

        public abstract iObject Bind(ArgumentBundle arguments);
    }

    internal class PrefixRequiredParameterBinder : ParameterBinder
    {
        public PrefixRequiredParameterBinder(int parameterIndex, CallInfo callInfo, MethodInformation methodInformation)
            : base(parameterIndex, callInfo, methodInformation)
        { }

        public override iObject Bind(ArgumentBundle arguments) => arguments.Splat[ParameterIndex];
    }

    internal class OptionalParameterBinder : ParameterBinder
    {
        public OptionalParameterBinder(int parameterIndex, CallInfo callInfo, MethodInformation methodInformation)
            : base(parameterIndex, callInfo, methodInformation)
        { }

        public override iObject Bind(ArgumentBundle arguments)
        {
            if(ParameterIndex < arguments.Splat.Count)
            {
                return arguments.Splat[ParameterIndex];
            }

            var parameter = MethodInformation.MethodInfo.GetParameters()[ParameterIndex];
            var defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;
            return Object.Box(defaultValue);
        }
    }
}