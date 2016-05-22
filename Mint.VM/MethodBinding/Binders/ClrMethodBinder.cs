using Mint.Reflection;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Binders
{
    public sealed class ClrMethodBinder : BaseMethodBinder
    {
        private static readonly MethodInfo INVALID_CONVERSION_METHOD = Reflector.Method(
            () => InvalidConversionMessage(default(MethodInformation[]), default(iObject[]))
        );

        private static readonly MethodInfo METHOD_BUNDLE = Reflector<CallInfo>.Method(
        _ => _.Bundle(default(iObject[]))
        );

        private static readonly ConstructorInfo CTOR_ARGERROR = Reflector.Ctor<ArgumentError>(typeof(string));
        private static readonly ConstructorInfo CTOR_TYPEERROR = Reflector.Ctor<TypeError>(typeof(string));

        private readonly MethodInformation[] methodInformations;

        public ClrMethodBinder(Symbol name, Module owner, MethodInfo method, Visibility visibility = Visibility.Public)
            : base(name, owner, visibility)
        {
            methodInformations = GetOverloads(method);
            Debug.Assert(methodInformations.Length != 0);
            Arity = CalculateArity();
        }

        private ClrMethodBinder(Symbol newName, ClrMethodBinder other)
            : base(newName, other)
        {
            methodInformations = (MethodInformation[]) other.methodInformations.Clone();
        }

        private static MethodInformation[] GetOverloads(MethodInfo method)
        {
            var methods =
                from m in method.DeclaringType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                where m.Name == method.Name
                select m
            ;

            if(method.IsStatic && !method.IsDefined(typeof(ExtensionAttribute)))
            {
                methods = methods.Concat(new[] { method });
            }

            return methods.Concat(method.GetExtensionOverloads()).Select(_ => new MethodInformation(_)).ToArray();
        }

        private Arity CalculateArity() =>
            methodInformations.Select(_ => _.ParameterInformation.Arity).Aggregate((left, right) => left.Merge(right));

        public override MethodBinder Alias(Symbol newName) => new ClrMethodBinder(newName, this);

        public override Expression Bind(InvocationInfo invocationInfo)
        {
            var length = invocationInfo.CallInfo.Arity;
            var filteredInfos = methodInformations.Where(_ => _.ParameterInformation.Arity.Include(length)).ToArray();

            if(filteredInfos.Length == 0)
            {
                return ThrowArgumentErrorExpression(length);
            }
            
            var bundle = Variable(typeof(ArgumentBundle), "bundle");
            var returnTarget = Label(typeof(iObject), "return");

            var bundleInfo = new InvocationInfo(invocationInfo.CallInfo, invocationInfo.Instance, bundle);

            var body = filteredInfos.Select(info =>
                new ClrMethodInvocationEmitter(info, bundleInfo, returnTarget).Bind()
            );

            var createBundleExpression = Call(
                Constant(invocationInfo.CallInfo),
                METHOD_BUNDLE,
                invocationInfo.Arguments
            );

            var bundleAssignExpression = Assign(bundle, createBundleExpression);
            var throwExpression = ThrowTypeExpression(invocationInfo.Arguments, filteredInfos);
            var returnExpression = Label(returnTarget, throwExpression);

            return Block(typeof(iObject), new[] { bundle },
                new[] { bundleAssignExpression }
                .Concat(body)
                .Concat(new[] { returnExpression })
            );
        }

        private UnaryExpression ThrowArgumentErrorExpression(int length)
        {
            return Throw(
                New(
                    CTOR_ARGERROR,
                    Constant($"wrong number of arguments (given {length}, expected {Arity})")
                    ),
                typeof(iObject)
                );
        }

        private static Expression ThrowTypeExpression(Expression arguments, MethodInformation[] filteredInfos)
        {
            return Throw(
                New(
                    CTOR_TYPEERROR,
                    Call(INVALID_CONVERSION_METHOD, Constant(filteredInfos), arguments)
                    ), typeof(iObject)
                );
        }

        private static string InvalidConversionMessage(MethodInformation[] infos, iObject[] args)
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
    }
}