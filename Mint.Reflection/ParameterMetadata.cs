using Mint.Reflection.Parameters;
using Mint.Reflection.Parameters.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mint.Reflection
{
    public class ParameterMetadata : AttributeMetadata
    {
        public ParameterInfo Parameter { get; }

        public string Name { get; }

        public int Position { get; }
        
        public bool IsOptional => HasAttribute<OptionalAttribute>();

        public bool IsRest => HasAttribute<RestAttribute>();

        public bool IsKey => HasAttribute<KeyAttribute>();

        public bool IsKeyRequired => IsKey && !IsOptional && !IsRest;

        public bool IsKeyOptional => IsKey && IsOptional;

        public bool IsKeyRest => IsKey && IsRest;

        public bool IsBlock => HasAttribute<BlockAttribute>();

        public bool IsRequired => !IsOptional && !IsRest && !IsBlock;

        public ParameterKind Kind { get; }

        public ParameterMetadata(ParameterInfo parameter,
                                 string name = null,
                                 int? position = null,
                                 IEnumerable<Attribute> customAttributes = null)
            : base(parameter.GetCustomAttributes(), customAttributes)
        {
            if(parameter == null) throw new ArgumentNullException(nameof(parameter));

            Parameter = parameter;

            Name = name ?? Parameter.Name;
            if(Name == null) throw new ArgumentNullException(nameof(name));

            Position = position ?? Parameter.Position;
            Kind = GetParameterKind();
        }

        public ParameterMetadata(ParameterInfo parameter,
                                 string name = null,
                                 int? position = null,
                                 params Attribute[] customAttributes)
            : this(parameter, name, position, (IEnumerable<Attribute>) customAttributes)
        { }

        public bool HasAttribute<T>() =>
            Parameter.IsDefined(typeof(T), false) || Attributes.Any(a => a.GetType() == typeof(T));

        private ParameterKind GetParameterKind()
        {
            if(IsBlock)
            {
                return ParameterKind.Block;
            }

            if(IsOptional)
            {
                return IsKey ? ParameterKind.KeyOptional : ParameterKind.Optional;
            }

            if(IsRest)
            {
                return IsKey ? ParameterKind.KeyRest : ParameterKind.Rest;
            }

            return IsKey ? ParameterKind.KeyRequired : ParameterKind.Required;
        }
    }
}
