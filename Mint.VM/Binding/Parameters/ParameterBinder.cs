using Mint.Binding.Arguments;
using Mint.Reflection.Parameters;
using System.Linq;
using System.Reflection;

namespace Mint.Binding.Parameters
{
    public abstract class ParameterBinder
    {
        public ParameterInfo Parameter { get; }
        public ParameterInformation ParameterCounter { get; }

        public ParameterBinder(ParameterInfo parameter, ParameterInformation counter)
        {
            Parameter = parameter;
            ParameterCounter = counter;
        }

        public abstract iObject Bind(ArgumentBundle bundle);
    }

    internal class PrefixRequiredParameterBinder : ParameterBinder
    {
        public PrefixRequiredParameterBinder(ParameterInfo parameter, ParameterInformation counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            if(Parameter.Position >= bundle.Splat.Count)
            {
                throw new ArgumentError(
                    "required parameter `{Parameter.Name}' with index {Parameter.Position} not passed");
            }

            return bundle.Splat[Parameter.Position];
        }
    }

    internal class OptionalParameterBinder : ParameterBinder
    {
        public OptionalParameterBinder(ParameterInfo parameter, ParameterInformation counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            if(Parameter.Position < bundle.Splat.Count)
            {
                return bundle.Splat[Parameter.Position];
            }

            var defaultValue = Parameter.HasDefaultValue ? Parameter.DefaultValue : null;
            return Object.Box(defaultValue);
        }
    }

    internal class SuffixRequiredParameterBinder : ParameterBinder
    {
        public SuffixRequiredParameterBinder(ParameterInfo parameter, ParameterInformation counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            var numParameters = CountParameters();
            var splatPositionFromEnd = Parameter.Position + 1 - numParameters;
            var splatPositionFromStart = bundle.Splat.Count - splatPositionFromEnd;

            if(splatPositionFromStart >= bundle.Splat.Count)
            {
                throw new ArgumentError(
                    "required parameter `{Parameter.Name}' with index {Parameter.Position} not passed");
            }

            return bundle.Splat[splatPositionFromStart];
        }

        private int CountParameters() =>
            ParameterCounter.PrefixRequired
            + ParameterCounter.Optional
            + (ParameterCounter.HasRest ? 1 : 0)
            + ParameterCounter.SuffixRequired;
    }

    internal class RestParameterBinder : ParameterBinder
    {
        public RestParameterBinder(ParameterInfo parameter, ParameterInformation counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            var beginPosition = ParameterCounter.PrefixRequired + ParameterCounter.Optional;
            var endPosition = bundle.Splat.Count - ParameterCounter.SuffixRequired;

            var result = new Array();

            for(var i = beginPosition; i < endPosition; i++)
            {
                result.Add(bundle.Splat[i]);
            }

            var hasKeyArguments = bundle.CallInfo.Arguments.Any(a => a == ArgumentKind.Key || a == ArgumentKind.KeyRest);
            var hasKeyRestParameter = ParameterCounter.HasKeyRest;
            var restIncludesKeyRest = hasKeyArguments != hasKeyRestParameter;

            if(restIncludesKeyRest)
            {
                result.Add(new Hash(bundle.Keys));
            }

            return result;
        }
    }

    internal class KeyRequiredParameterBinder : ParameterBinder
    {
        public KeyRequiredParameterBinder(ParameterInfo parameter, ParameterInformation counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class KeyOptionalParameterBinder : ParameterBinder
    {
        public KeyOptionalParameterBinder(ParameterInfo parameter, ParameterInformation counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class KeyRestParameterBinder : ParameterBinder
    {
        public KeyRestParameterBinder(ParameterInfo parameter, ParameterInformation counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class BlockParameterBinder : ParameterBinder
    {
        public BlockParameterBinder(ParameterInfo parameter, ParameterInformation counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle) => bundle.Block ?? new NilClass();
    }
}