using Mint.MethodBinding;
using System.Reflection;

namespace Mint
{
    public static class ParameterInfoExtensions
    {
        public static bool IsOptional(this ParameterInfo parameterInfo) => parameterInfo.HasAttribute<OptionalAttribute>();

        public static bool IsRest(this ParameterInfo parameterInfo) => parameterInfo.HasAttribute<RestAttribute>();

        public static bool IsKey(this ParameterInfo parameterInfo) => parameterInfo.HasAttribute<KeyAttribute>();

        public static bool IsBlock(this ParameterInfo parameterInfo) => parameterInfo.HasAttribute<BlockAttribute>();

        public static bool IsKeyRest(this ParameterInfo parameterInfo) => parameterInfo.IsKey() && parameterInfo.IsRest();

        private static bool HasAttribute<T>(this ICustomAttributeProvider parameterInfo) => parameterInfo.IsDefined(typeof(T), false);
    }
}