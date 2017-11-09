using Mint.Reflection.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mint.Reflection
{
    public class MethodMetadata : AttributeMetadata
    {
        public MethodMetadata(MethodInfo method,
                              string name = null,
                              bool? isStatic = null,
                              bool? hasInstance = null,
                              IEnumerable<ParameterMetadata> parameters = null,
                              IEnumerable<Attribute> customAttributes = null)
            : base(method.GetCustomAttributes(), customAttributes)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Name = name ?? Method.Name ?? throw new ArgumentNullException(nameof(name));

            IsStatic = isStatic ?? Method.IsStatic;
            HasInstance = hasInstance ?? IsStatic;
            HasClosure = Method.GetParameters().FirstOrDefault()?.ParameterType == typeof(Closure);
            ParameterOffset = (HasInstance ? 1 : 0) + (HasClosure ? 1 : 0);
            Parameters = InitializeParameters(parameters, ParameterOffset);
            ValidateParameters();
        }


        public MethodInfo Method { get; }
        public string Name { get; }
        public bool IsStatic { get; }
        public bool HasInstance { get; }
        public bool HasClosure { get; }
        public int ParameterOffset { get; }
        public IList<ParameterMetadata> Parameters { get; } // Doesn't store Closure or instance parameter


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


        private IList<ParameterMetadata> InitializeParameters(IEnumerable<ParameterMetadata> parameters, int offset)
        {
            if(parameters == null)
            {
                parameters = Method.GetParameters().Skip(offset)
                    .Select((p, i) => new ParameterMetadata(p, position: i));
            }

            return parameters.ToList().AsReadOnly();
        }


        private void ValidateParameters()
        {
            if(Parameters.Count == 0)
            {
                return;
            }

            var parameter = Parameters.FirstOrDefault(p => p.Parameter.Member != Method);

            if(parameter != null)
            {
                throw new ArgumentException(
                    $"parameter ({parameter.Parameter}) doesn't belong to passed method ({Method})");
            }

            var numDistinctParameters = Parameters.Select(p => p.Parameter).Distinct().Count();

            if(Parameters.Count != numDistinctParameters)
            {
                throw new ArgumentException("parameter list contains duplicates");
            }
        }


        public bool HasAttribute<T>()
            => Method.IsDefined(typeof(T), false) || Attributes.Any(a => a is T);
    }
}
