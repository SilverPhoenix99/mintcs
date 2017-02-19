using System;
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

            var methods =
                from m in method.Method.ReflectedType.GetMethods(Instance | NonPublic | Public)
                where m.Name == method.Name
                select new MethodMetadata(m)
            ;

            if(method.IsStatic && !method.Method.IsDefined(typeof(ExtensionAttribute)))
            {
                methods = methods.Concat(new[] { method });
            }

            var overloads = method.Method.GetExtensionOverloads().Select(m => new MethodMetadata(m));
            return methods.Concat(overloads).ToArray();
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