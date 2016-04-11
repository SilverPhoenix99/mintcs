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

        private Info[] GetOverloads(MethodInfo method)
        {
            var methods = GetExtensionMethods(method);

            throw new NotImplementedException();
        }

        private Range CalculateArity()
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<MethodInfo> GetExtensionMethods(MethodInfo method)
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                   from type in assembly.GetTypes()
                   where type.IsSealed
                      && !type.IsGenericType
                      && !type.IsNested
                      && type.IsDefined(typeof(ExtensionAttribute), false)
                   from m in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                   where m.Name == method.Name
                             && m.IsDefined(typeof(ExtensionAttribute), false)
                             //&& m.GetParameters()[0].ParameterType.IsAssignableFrom(method.DeclaringType)
                   select m
            ;
        }
    }
}