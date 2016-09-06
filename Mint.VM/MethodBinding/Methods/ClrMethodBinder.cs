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
     *         case <InstanceCallEmitter code>;
     *         
     *         default:
     *             new TypeError("no implicit conversion exists");
     *     }
     * }
     */
    public sealed partial class ClrMethodBinder : BaseMethodBinder
    {
        private MethodInfo[] MethodInfos { get; }

        public ClrMethodBinder(Symbol name, Module owner, MethodInfo method, Visibility visibility = Visibility.Public)
            : base(name, owner, visibility)
        {
            if(method.IsDynamicallyGenerated())
            {
                throw new ArgumentException("Method cannot be dynamically generated. Use DelegateMethodBinder instead.");
            }

            MethodInfos = GetOverloads(method);
            Debug.Assert(MethodInfos.Length != 0);
            Arity = CalculateArity();
        }

        private ClrMethodBinder(Symbol newName, ClrMethodBinder other)
            : base(newName, other)
        {
            MethodInfos = (MethodInfo[]) other.MethodInfos.Clone();
        }

        private static MethodInfo[] GetOverloads(MethodInfo method)
        {
            Debug.Assert(method.ReflectedType != null, "method.ReflectedType != null");

            var methods =
                from m in method.ReflectedType.GetMethods(Instance | NonPublic | Public)
                where m.Name == method.Name
                select m
            ;

            if(method.IsStatic && !method.IsDefined(typeof(ExtensionAttribute)))
            {
                methods = methods.Concat(new[] { method });
            }

            return methods.Concat(method.GetExtensionOverloads()).ToArray();
        }

        private Arity CalculateArity() =>
            MethodInfos.Select(_ => _.GetParameterCounter().Arity).Aggregate((left, right) => left.Merge(right));

        public override MethodBinder Duplicate(Symbol newName) => new ClrMethodBinder(newName, this);

        public override Expression Bind(CallFrameBinder frame)
        {
            var argumentsArray = Variable(typeof(iObject[]), "arguments");
            var cases = MethodInfos.Select(info => CreateCallEmitter(info, frame, argumentsArray).Bind());

            var defaultCase = Throw(Expressions.ThrowInvalidConversion(), typeof(iObject));

            return Block(
                typeof(iObject),
                new[] { argumentsArray },
                Switch(typeof(iObject), Constant(true), defaultCase, null, cases)
            );
        }

        private static CallEmitter CreateCallEmitter(
            MethodInfo info,
            CallFrameBinder frame,
            ParameterExpression argumentsArray
        ) =>
            info.IsStatic ? new StaticCallEmitter(info, frame, argumentsArray)
                          : new InstanceCallEmitter(info, frame, argumentsArray);
        
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