using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using static System.Reflection.BindingFlags;

namespace Mint.Reflection
{
    public static class TypeExtensions
    {
        public static IEnumerable<MethodInfo> GetExtensionOverloads(this Type type, string methodName)
        {
            return
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from t in assembly.GetTypes()
                where t.IsSealed
                      && !t.IsGenericType
                      && !t.IsNested
                      && t.IsDefined(typeof(ExtensionAttribute), false)
                from method in t.GetMethods(Static | NonPublic | Public)
                where method.IsDefined(typeof(ExtensionAttribute), false)
                      && method.Name == methodName
                      && method.GetParameters()[0].IsAssignableFrom(type)
                select method
            ;
        }


        public static IEnumerable<MethodInfo> GetMethodOverloads(this Type type, string methodName)
        {
            return
                from method in type.GetMethods(Instance | Static | NonPublic | Public)
                where method.Name == methodName
                      && !method.IsDefined(typeof(ExtensionAttribute), false)
                select method
            ;
        }
    }
}