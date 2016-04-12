using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mint.Test
{
    internal static class TestExtensionReflection
    {
        public static void MainTest()
        {
            var mi = typeof(iObject).GetMethod("Inspect");
            var w = new Stopwatch();

            for(var i = 0; i < 30; i++)
            {
                w.Start();

                var q = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetTypes()
                        where type.IsSealed
                           && !type.IsGenericType
                           && !type.IsNested
                           && type.IsDefined(typeof(ExtensionAttribute), false)
                        from m in type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                        where m.IsDefined(typeof(ExtensionAttribute), false)
                           && m.Name == mi.Name
                           && Matches(m.GetParameters()[0], mi.DeclaringType)
                        select m;

                var a = q.ToArray();

                w.Stop();
                Console.WriteLine(w.ElapsedMilliseconds);
                w.Reset();
            }
        }

        private static bool Matches(ParameterInfo info, Type declaringType)
        {
            if(!info.ParameterType.IsGenericParameter)
            {
                // return : info.ParameterType is == or superclass of declaringType?
                var matches = info.ParameterType.IsAssignableFrom(declaringType);
                return matches;
            }

            var constraints = info.ParameterType.GetGenericParameterConstraints();
            return constraints.Length == 0 || constraints.Any(type => type.IsAssignableFrom(declaringType));
        }
    }

    internal static class X
    {
        /*
        public static string Inspect(this iObject obj, bool full) =>
            full ? obj.CalculatedClass.FullName : obj.CalculatedClass.Name.ToString();

        public static string Inspect<T>(this T obj, string prefix) where T : iObject =>
            prefix + obj.Inspect();

        public static string Inspect<T>(this T obj, int a, int b) => $"{a}..{b} {obj}";

        public static string Inspect(this object obj, int a, string b) => $"{a}..{b} {obj}";

        public static string Inspect<T>(this T obj, string a, int b) where T : Condition => $"{a}..{b} {obj}";

        public static string Inspect(this MethodInfo obj, string a, char b) => $"{a}..{b} {obj}";
        */
    }
}
