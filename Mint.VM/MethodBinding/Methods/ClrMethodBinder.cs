using System;
using System.Collections.Generic;
using Mint.Reflection;
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
        private MethodMetadata Method { get; }

        public ClrMethodBinder(Symbol name,
                               Module owner,
                               MethodMetadata method,
                               Visibility visibility = Visibility.Public)
            : base(name, owner, visibility)
        {
            if(method == null) throw new ArgumentNullException(nameof(method));

            if(method.Method.IsDynamicallyGenerated())
            {
                throw new ArgumentException("Method cannot be dynamically generated. Use DelegateMethodBinder instead.");
            }

            Method = method;
        }

        private ClrMethodBinder(Symbol newName, ClrMethodBinder other)
            : base(newName, other)
        {
            Method = other.Method;
        }

        public override MethodBinder Duplicate(Symbol newName) => new ClrMethodBinder(newName, this);

        public override Expression Bind(CallFrameBinder frame)
        {
            var argumentsArray = Variable(typeof(iObject[]), "arguments");
            var locator = new MethodOverloadLocator(Method, frame.InstanceType);
            var methods = locator.GetOverloads();
            var cases = methods.Select(method => CreateCallEmitter(method, frame, argumentsArray).Bind());

            var defaultCase = Throw(Expressions.ThrowInvalidConversion(), typeof(iObject));

            return Block(
                typeof(iObject),
                new[] { argumentsArray },
                Switch(typeof(iObject), Constant(true), defaultCase, null, cases)
            );
        }

        private static CallEmitter CreateCallEmitter(MethodMetadata method,
                                                     CallFrameBinder frame,
                                                     ParameterExpression argumentsArray) =>
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

        private class MethodOverloadLocator
        {
            public MethodMetadata Prototype { get; }

            public Type InstanceType { get; }

            public MethodOverloadLocator(MethodMetadata prototype, Type instanceType)
            {
                if(prototype == null) throw new ArgumentNullException(nameof(prototype));
                if(instanceType == null) throw new ArgumentNullException(nameof(instanceType));

                Prototype = prototype;
                InstanceType = instanceType;
            }

            public IEnumerable<MethodMetadata> GetOverloads()
            {
                var methods = GetMethodOverloads();
                var extensionMethods = GetExtensionOverloads();

                var methodList = methods.Concat(extensionMethods).Select(m => new MethodMetadata(m)).ToList();

                if(!methodList.Exists(_ => _.Method == Prototype.Method))
                {
                    methodList.Add(Prototype);
                }

                return methodList;
            }

            private IEnumerable<MethodInfo> GetMethodOverloads()
            {
                return
                    from method in InstanceType.GetMethods(Instance | Static | NonPublic | Public)
                    where method.Name == Prototype.Name
                       && !method.IsDefined(typeof(ExtensionAttribute), false)
                    select method
                ;
            }

            private IEnumerable<MethodInfo> GetExtensionOverloads()
            {
                return
                    from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where type.IsSealed
                       && !type.IsGenericType
                       && !type.IsNested
                       && type.IsDefined(typeof(ExtensionAttribute), false)
                    from method in type.GetMethods(Static | NonPublic | Public)
                    where method.IsDefined(typeof(ExtensionAttribute), false)
                       && method.Name == Prototype.Name
                       && Matches(method.GetParameters()[0])
                    select method
                ;
            }

            private bool Matches(ParameterInfo info)
            {
                if(!info.ParameterType.IsGenericParameter)
                {
                    // return : info.ParameterType is == or superclass of declaringType?
                    var matches = info.ParameterType.IsAssignableFrom(InstanceType);
                    return matches;
                }

                var constraints = info.ParameterType.GetGenericParameterConstraints();
                return constraints.Length == 0 || constraints.Any(type => type.IsAssignableFrom(InstanceType));
            }
        }
    }
}