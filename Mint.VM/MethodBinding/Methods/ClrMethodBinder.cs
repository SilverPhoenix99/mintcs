using Mint.MethodBinding.Arguments;
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
     *     if(@Condition.Valid)
     *     {
     *         <CallEmitter code>
     *     }
     *     else
     *     {
     *         @CallSite.BundledCall = @CallSite.CallCompiler.Compile();
     *         return @CallSite.BundledCall($instance, $bundle);
     *     }
     * }
     */
    public sealed partial class ClrMethodBinder : BaseMethodBinder
    {
        private readonly MethodInfo[] methodInfos;

        public ClrMethodBinder(Symbol name, Module owner, MethodInfo method, Visibility visibility = Visibility.Public)
            : base(name, owner, visibility)
        {
            methodInfos = GetOverloads(method);
            Debug.Assert(methodInfos.Length != 0);
            Arity = CalculateArity();
        }

        private ClrMethodBinder(Symbol newName, ClrMethodBinder other)
            : base(newName, other)
        {
            methodInfos = (MethodInfo[]) other.methodInfos.Clone();
        }

        private static MethodInfo[] GetOverloads(MethodInfo method)
        {
            var methods =
                from m in method.DeclaringType.GetMethods(Instance | NonPublic | Public)
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
            methodInfos.Select(_ => _.GetParameterCounter().Arity).Aggregate((left, right) => left.Merge(right));

        public override MethodBinder Duplicate(Symbol newName) => new ClrMethodBinder(newName, this);

        public override Expression Bind(CallFrameBinder frame)
        {
            var length = frame.CallSite.ArgumentKinds.Count;
            var methods = methodInfos.Where(_ => _.GetParameterCounter().Arity.Include(length)).ToArray();

            if(methods.Length == 0)
            {
                return ThrowArgumentErrorExpression(length);
            }

            var bundle = Variable(typeof(ArgumentBundle), "bundle");
            var returnTarget = Label(typeof(iObject), "return");

            var bundledFrame = new CallFrameBinder(frame.CallSite, frame.Instance, bundle);

            var body = methods.Select(info => new CallEmitter(info, bundledFrame, returnTarget).Bind());

            var createBundleExpression = CallSite.Expressions.CreateBundle(Constant(frame.CallSite), frame.Arguments);

            var bundleAssignExpression = Assign(bundle, createBundleExpression);
            var throwExpression = ThrowTypeExpression(frame.Arguments, methods);
            var returnExpression = Label(returnTarget, throwExpression);

            var result = Block(typeof(iObject), new[] { bundle },
                new[] { bundleAssignExpression }
                .Concat(body)
                .Concat(new[] { returnExpression })
            );

            System.Console.WriteLine("--------");
            System.Console.WriteLine();
            System.Console.WriteLine(result.Inspect());
            System.Console.WriteLine();
            System.Console.WriteLine("--------");
            System.Console.WriteLine();

            return result;
        }

        private UnaryExpression ThrowArgumentErrorExpression(int length)
        {
            return Throw(
                ArgumentError.Expressions.New(
                    Constant($"wrong number of arguments (given {length}, expected {Arity})")
                ),
                typeof(iObject)
            );
        }

        private static Expression ThrowTypeExpression(Expression arguments, MethodInfo[] methods)
        {
            return Throw(
                TypeError.Expressions.New(
                    Expressions.InvalidConversionMessage(Constant(methods), arguments)
                ),
                typeof(iObject)
            );
        }

        private static string InvalidConversionMessage(MethodInfo[] methods, iObject[] arguments)
        {
            // TODO

            //for(var i = 0; i < arguments.Length; i++)
            //{
            //    var arg = arguments[i];
            //    var types = methodInformations.Select(_ => _.MethodInfo.GetParameters()[i]).an;
            //}

            //msg = "argument {index}: no implicit conversion of {type} to {string.Join(" or ", types)}";
            return "no implicit conversion exists";
        }

        public static class Reflection
        {
            public static readonly MethodInfo InvalidConversionMessage = Reflector.Method(
                () => InvalidConversionMessage(default(MethodInfo[]), default(iObject[]))
            );
        }

        public static class Expressions
        {
            public static MethodCallExpression InvalidConversionMessage(Expression methods, Expression arguments) =>
                Expression.Call(Reflection.InvalidConversionMessage, methods, arguments);
        }
    }
}