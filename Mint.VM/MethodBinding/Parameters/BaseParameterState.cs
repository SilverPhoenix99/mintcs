using System;
using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    internal abstract class BaseParameterState : ParameterState
    {
        protected readonly ParameterInformation ParameterInformation;

        protected BaseParameterState(ParameterInformation parameterInformation)
        {
            ParameterInformation = parameterInformation;
        }

        public abstract ParameterState Parse(ParameterInfo info);

        protected ParameterState ParseInfoWith<T>(ParameterInfo info) where T : ParameterState =>
            ((ParameterState) Activator.CreateInstance(typeof(T), ParameterInformation)).Parse(info);

        protected static ParameterState InvalidParameterError(ParameterInfo info)
        {
            throw new InvalidParameterError($"Parameter `{info.Name}' has an invalid parameter kind.");
        }

        protected static void DuplicateParameterError(string type, ParameterInfo info)
        {
            throw new InvalidParameterError($"Duplicate {type} parameter: `{info.Name}'");
        }
    }
}