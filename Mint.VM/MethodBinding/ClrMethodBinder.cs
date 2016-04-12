using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mint.MethodBinding
{
    public sealed partial class ClrMethodBinder : BaseMethodBinder
    {
        private readonly Info[] infos;

        public ClrMethodBinder(Symbol name, Module owner, MethodInfo method)
            : base(name, owner)
        {
            infos = GetOverloads(method);
            Arity = CalculateArity();
        }

        private ClrMethodBinder(ClrMethodBinder other, bool copyValidation)
            : base(other, copyValidation)
        {
            infos = (Info[]) other.infos.Clone();
        }

        public override Expression Bind(Expression instance, IEnumerable<Expression> args)
        {
            throw new NotImplementedException();
        }

        public override MethodBinder Duplicate(bool copyValidation) => new ClrMethodBinder(this, copyValidation);

        private static Info[] GetOverloads(MethodInfo method)
        {
            var methods =
                from m in method.DeclaringType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                where m.Name == method.Name
                select m
            ;

            return methods.Concat(GetExtensionMethods(method)).Select(_ => new Info(_)).ToArray();
        }

        private Range CalculateArity()
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<MethodInfo> GetExtensionMethods(MethodInfo method)
        {
            return
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.IsSealed
                   && !type.IsGenericType
                   && !type.IsNested
                   && type.IsDefined(typeof(ExtensionAttribute), false)
                from m in type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                where m.IsDefined(typeof(ExtensionAttribute), false)
                   && m.Name == method.Name
                   && Matches(m.GetParameters()[0], method.DeclaringType)
                select m;
            ;
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
}