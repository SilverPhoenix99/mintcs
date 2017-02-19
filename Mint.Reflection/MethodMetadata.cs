using Mint.Reflection.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mint.Reflection
{
    public class AttributeMetadata
    {
        public IList<Attribute> Attributes { get; }

        public AttributeMetadata(params IEnumerable<Attribute>[] attributesCollection)
        {
            Attributes = attributesCollection.Where(_ => _ != null).SelectMany(_ => _).ToList();
        }
    }

    public class MethodMetadata : AttributeMetadata
    {
        public MethodInfo Method { get; }

        public string Name { get; }

        public bool IsStatic { get; }

        public bool HasInstance { get; }

        public bool HasClosure { get; }

        public int ParameterOffset { get; }

        // Doesn't store Closure or instante parameters
        public IList<ParameterMetadata> Parameters { get; }

        public ParameterCounter ParameterCounter
        {
            get
            {
                var parameterCounter = Attributes.OfType<ParameterCounter>().FirstOrDefault();

                if(parameterCounter == null)
                {
                    parameterCounter = new ParameterCounter(this);
                    Attributes.Add(parameterCounter);
                }

                return parameterCounter;
            }
        }

        public MethodMetadata(MethodInfo method,
                              string name = null,
                              bool? isStatic = null,
                              bool? hasInstance = null,
                              IEnumerable<ParameterMetadata> parameters = null,
                              IEnumerable<Attribute> customAttributes = null)
            : base(method.GetCustomAttributes(), customAttributes)
        {
            if(method == null) throw new ArgumentNullException(nameof(method));

            Method = method;

            Name = name ?? Method.Name;
            if(Name == null) throw new ArgumentNullException(nameof(name));

            IsStatic = isStatic ?? Method.IsStatic;
            HasInstance = hasInstance ?? IsStatic;
            HasClosure = Method.GetParameters().FirstOrDefault()?.ParameterType == typeof(Closure);
            ParameterOffset = (HasInstance ? 1 : 0) + (HasClosure ? 1 : 0);
            Parameters = InitializeParameters(parameters, ParameterOffset);
        }

        private IList<ParameterMetadata> InitializeParameters(
            IEnumerable<ParameterMetadata> parameters, int offset)
        {
            if(parameters == null)
            {
                parameters = Method.GetParameters().Skip(offset)
                    .Select((p, i) => new ParameterMetadata(p, position: i));
            }

            var parameterList = parameters.ToList().AsReadOnly();

            ValidateParameters(parameterList);

            return parameterList;
        }

        private void ValidateParameters(IList<ParameterMetadata> parameters)
        {
            if(parameters.Count == 0)
            {
                return;
            }

            var parameter = parameters.FirstOrDefault(p => p.Parameter.Member != Method);

            if(parameter != null)
            {
                throw new ArgumentException(
                    $"parameter ({parameter.Parameter}) doesn't belong to passed method ({Method})");
            }

            var numDistinctParameters = parameters.Select(p => p.Parameter).Distinct().Count();

            if(parameters.Count != numDistinctParameters)
            {
                throw new ArgumentException("parameter list contains duplicates");
            }
        }
    }
}
