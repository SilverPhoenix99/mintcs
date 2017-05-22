using System;
using System.Collections.Generic;
using Mint.Reflection;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

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
                               Module definer,
                               MethodMetadata method,
                               Module caller = null,
                               Visibility visibility = Visibility.Public)
            : base(name, definer, caller, visibility)
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
            var methods = GetOverloads(frame);
            var cases = methods.Select(method => CreateCallEmitter(method, frame, argumentsArray).Bind());

            var defaultCase = Throw(Expressions.ThrowInvalidConversion(), typeof(iObject));

            return Block(
                typeof(iObject),
                new[] { argumentsArray },
                Switch(typeof(iObject), Constant(true), defaultCase, null, cases)
            );
        }

        private IEnumerable<MethodMetadata> GetOverloads(CallFrameBinder frame)
        {
            var methods = frame.InstanceType.GetMethodOverloads(Method.Name);
            var extensionMethods = frame.InstanceType.GetExtensionOverloads(Method.Name);

            var methodList = methods.Concat(extensionMethods).Select(m => new MethodMetadata(m)).ToList();

            if(!methodList.Exists(_ => _.Method == Method.Method))
            {
                methodList.Add(Method);
            }

            return methodList;
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
    }
}