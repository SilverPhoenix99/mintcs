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

        protected ParameterState Match(bool condition, Type type) =>
            condition ? (ParameterState) Activator.CreateInstance(type, ParameterInformation) : null;

        protected static ParameterState Match(bool condition, Func<ParameterState> parseFunction) =>
            condition ? parseFunction() : null;

        protected ParameterState Match(bool condition, Action parseFunction) =>
            Match(condition, () => { parseFunction(); return this; });

        protected static ParameterState InvalidParameter(ParameterInfo info)
        {
            throw new InvalidParameterError($"Parameter `{info.Name}' has an invalid parameter kind.");
        }

        protected static ParameterState DuplicateParameter(string type, ParameterInfo info)
        {
            throw new InvalidParameterError($"Duplicate {type} parameter: `{info.Name}'");
        }
    }
}