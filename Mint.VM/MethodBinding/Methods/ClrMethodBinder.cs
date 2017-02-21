using System;
using System.Collections.Generic;
using Mint.Reflection;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using static System.Linq.Expressions.Expression;
using static System.Reflection.BindingFlags;

namespace Mint.MethodBinding.Methods
{
    /*
     * Generated Stub:
     *
     * (iObject $instance, ArgumentBundle $bundle) => {
     *
     *     switch
     *     {
     *         case <CallEmitter code>;
     *         
     *         default:
     *             new TypeError("no implicit conversion exists");
     *     }
     * }
     */
    public sealed partial class ClrMethodBinder : BaseMethodBinder
    {
        private MethodMetadata[] Methods { get; }

        public ClrMethodBinder(Symbol name,
                               Module owner,
                               MethodMetadata method,
                               Visibility visibility = Visibility.Public)
            : base(name, owner, visibility)
        {
            if(method.Method.IsDynamicallyGenerated())
            {
                throw new ArgumentException("Method cannot be dynamically generated. Use DelegateMethodBinder instead.");
            }

            Methods = GetOverloads(method);
            Debug.Assert(Methods.Length != 0);
            Arity = CalculateArity();
        }

        private ClrMethodBinder(Symbol newName, ClrMethodBinder other)
            : base(newName, other)
        {
            Methods = (MethodMetadata[]) other.Methods.Clone();
        }

        private static MethodMetadata[] GetOverloads(MethodMetadata method)
        {
            Debug.Assert(method.Method.ReflectedType != null, "method.ReflectedType != null");

            var type = method.HasAttribute<ExtensionAttribute>()
                     ? method.Method.GetParameters()[0].ParameterType
                     : method.Method.ReflectedType;

            var methods = GetMethodOverloads(method.Name, type);
            var extensionMethods = GetExtensionOverloads(method.Name, type);

            return methods.Concat(extensionMethods).Select(m => new MethodMetadata(m)).ToArray();
        }

        private static IEnumerable<MethodInfo> GetMethodOverloads(string methodName, Type instanceType)
        {
            return
                from method in instanceType.GetMethods(Instance | Static | NonPublic | Public)
                where method.Name == methodName
                   && !method.IsDefined(typeof(ExtensionAttribute), false)
                select method
            ;
        }

        private static IEnumerable<MethodInfo> GetExtensionOverloads(string methodName, Type instanceType)
        {
            return
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.IsSealed
                   && !type.IsGenericType
                   && !type.IsNested
                   && type.IsDefined(typeof(ExtensionAttribute), false)
                from method in type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                where method.IsDefined(typeof(ExtensionAttribute), false)
                   && method.Name == methodName
                   && Matches(method.GetParameters()[0], instanceType)
                select method
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

        private Arity CalculateArity() =>
            Methods.Select(_ => _.ParameterCounter.Arity).Aggregate((left, right) => left.Merge(right));

        public override MethodBinder Duplicate(Symbol newName) => new ClrMethodBinder(newName, this);

        public override Expression Bind(CallFrameBinder frame)
        {
            var argumentsArray = Variable(typeof(iObject[]), "arguments");
            var cases = Methods.Select(method => CreateCallEmitter(method, frame, argumentsArray).Bind());

            var defaultCase = Throw(Expressions.ThrowInvalidConversion(), typeof(iObject));

            return Block(
                typeof(iObject),
                new[] { argumentsArray },
                Switch(typeof(iObject), Constant(true), defaultCase, null, cases)
            );
        }

        private static CallEmitter CreateCallEmitter(
            MethodMetadata method,
            CallFrameBinder frame,
            ParameterExpression argumentsArray
        ) =>
            method.IsStatic ? new StaticCallEmitter(method, frame, argumentsArray)
                          : new InstanceCallEmitter(method, frame, argumentsArray);
        
        private static Exception ThrowInvalidConversion()
        {
            // TODO

            //for(var i = 0; i < arguments.Length; i++)
            //{
            //    var arg = arguments[i];
            //    var types = methodInformations.Select(_ => _.MethodInfo.GetParameters()[i]).an;
            //}

            //msg = "argument {index}: no implicit conversion of {type} to {string.Join(" or ", types)}";
            return new TypeError("no implicit conversion exists");
        }

        public static class Reflection
        {
            public static readonly MethodInfo ThrowInvalidConversion = Reflector.Method(
                () => ThrowInvalidConversion()
            );
        }

        public static class Expressions
        {
            public static MethodCallExpression ThrowInvalidConversion() => Call(Reflection.ThrowInvalidConversion);
        }
    }
}