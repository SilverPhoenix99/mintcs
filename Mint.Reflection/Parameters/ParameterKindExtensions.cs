using System;
using Mint.Reflection.Parameters.Attributes;

namespace Mint.Reflection.Parameters
{
    public static class ParameterKindExtensions
    {
        public static Attribute[] GetAttributes(this ParameterKind kind)
        {
            switch(kind)
            {
                case ParameterKind.Optional:
                    return new Attribute[] { new OptionalAttribute() };

                case ParameterKind.Rest:
                    return new Attribute[] { new RestAttribute() };

                case ParameterKind.Block:
                    return new Attribute[] { new BlockAttribute() };

                case ParameterKind.KeyRequired:
                    return new Attribute[] { new KeyAttribute() };

                case ParameterKind.KeyOptional:
                    return new Attribute[] { new KeyAttribute(), new OptionalAttribute() };

                case ParameterKind.KeyRest:
                    return new Attribute[] { new KeyAttribute(), new RestAttribute() };

                default:
                    return Array.Empty<Attribute>();
            }
        }
    }
}
