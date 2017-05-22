using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mint.Reflection
{
    public static class MethodInfoExtensions
    {
        public static bool IsDynamicallyGenerated(this MethodBase methodInfo) =>
            methodInfo.ReflectedType?.IsDefined(typeof(CompilerGeneratedAttribute), false) ?? false;
    }
}
