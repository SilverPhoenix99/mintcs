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
        private IList<MethodMetadata> Methods { get; }

        public ClrMethodBinder(Symbol name,
                               Module owner,
                               IEnumerable<MethodMetadata> methods,
                               Module caller = null,
                               Visibility visibility = Visibility.Public)
            : base(name, owner, caller, visibility)
        {
            if(methods == null) throw new ArgumentNullException(nameof(methods));

            Methods = new List<MethodMetadata>(methods);

            if(Methods.Any(method => method.Method.IsDynamicallyGenerated()))
            {
                throw new ArgumentException("Method cannot be dynamically generated. Use DelegateMethodBinder instead.");
            }
        }
        
        private ClrMethodBinder(Symbol newName, ClrMethodBinder other)
            : base(newName, other)
        {
            Methods = new List<MethodMetadata>(other.Methods);
        }
        
        public override MethodBinder Duplicate(Symbol newName) => new ClrMethodBinder(newName, this);

        public override Expression Bind(CallFrameBinder frame)
        {
            var argumentsArray = Variable(typeof(iObject[]), "arguments");

            var cases = from method in Methods
                        select CreateCallEmitter(method, frame, argumentsArray).Bind();

            var defaultCase = Throw(Expressions.ThrowInvalidConversion(), typeof(iObject));

            return Block(
                typeof(iObject),
                new[] { argumentsArray },
                Switch(typeof(iObject), Constant(true), defaultCase, null, cases)
            );
        }
        
        private static CallEmitter CreateCallEmitter(MethodMetadata method,
                                                     CallFrameBinder frame,
                                                     ParameterExpression argumentsArray)
            => method.IsStatic
                ? new StaticCallEmitter(method, frame, argumentsArray)
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
            public static MethodCallExpression ThrowInvalidConversion()
                => Call(Reflection.ThrowInvalidConversion);
        }
    }
}