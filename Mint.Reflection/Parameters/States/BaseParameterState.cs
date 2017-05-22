using System;

namespace Mint.Reflection.Parameters
{
    internal abstract class BaseParameterState : ParameterState
    {
        protected BaseParameterState(ParameterCounter parameterCounter)
        {
            ParameterCounter = parameterCounter;
        }


        protected ParameterCounter ParameterCounter { get; }


        public abstract ParameterState Parse(ParameterMetadata parameter);


        protected ParameterState ParseInfoWith<T>(ParameterMetadata parameter)
            where T : ParameterState
            => ((ParameterState) Activator.CreateInstance(typeof(T), ParameterCounter)).Parse(parameter);


        protected static ParameterState InvalidParameterError(ParameterMetadata parameter)
             => throw new InvalidParameterError($"Parameter `{parameter.Name}' has an invalid parameter kind.");


        protected static void DuplicateParameterError(string type, ParameterMetadata parameter)
            => throw new InvalidParameterError($"Duplicate {type} parameter: `{parameter.Name}'");
    }
}