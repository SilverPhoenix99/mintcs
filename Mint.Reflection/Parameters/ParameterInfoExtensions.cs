using Mint.Reflection.Parameters.Attributes;
using System.Reflection;

namespace Mint.Reflection.Parameters
{
    public static class ParameterInfoExtensions
    {
        public static bool IsOptional(this ParameterInfo parameterInfo) =>
            parameterInfo.HasAttribute<OptionalAttribute>();

        public static bool IsRest(this ParameterInfo parameterInfo) => parameterInfo.HasAttribute<RestAttribute>();

        public static bool IsKey(this ParameterInfo parameterInfo) => parameterInfo.HasAttribute<KeyAttribute>();

        public static bool IsBlock(this ParameterInfo parameterInfo) => parameterInfo.HasAttribute<BlockAttribute>();

        public static bool IsKeyRest(this ParameterInfo parameterInfo) =>
            parameterInfo.IsKey() && parameterInfo.IsRest();

        public static bool IsRequired(this ParameterInfo parameterInfo) =>
            !parameterInfo.IsOptional()
            && !parameterInfo.IsRest()
            && !parameterInfo.IsBlock();

        private static bool HasAttribute<T>(this ICustomAttributeProvider parameterInfo) =>
            parameterInfo.IsDefined(typeof(T), false);

        public static ParameterKind GetParameterKind(this ParameterInfo parameterInfo)
        {
            if(parameterInfo.IsBlock())
            {
                return ParameterKind.Block;
            }

            if(parameterInfo.IsOptional())
            {
                return parameterInfo.IsKey() ? ParameterKind.KeyOptional : ParameterKind.Optional;
            }

            if(parameterInfo.IsRest())
            {
                return parameterInfo.IsKey() ? ParameterKind.KeyRest : ParameterKind.Rest;
            }

            return parameterInfo.IsKey() ? ParameterKind.KeyRequired : ParameterKind.Required;
        }
    }
}