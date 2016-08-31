using System;
using System.Reflection;

namespace Mint.Reflection.Parameters
{
    internal abstract class BaseParameterState : ParameterState
    {
        protected readonly ParameterCounter ParameterCounter;

        protected BaseParameterState(ParameterCounter parameterCounter)
        {
            ParameterCounter = parameterCounter;
        }

        public abstract ParameterState Parse(ParameterInfo info);

        protected ParameterState ParseInfoWith<T>(ParameterInfo info) where T : ParameterState =>
            ((ParameterState) Activator.CreateInstance(typeof(T), ParameterCounter)).Parse(info);

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