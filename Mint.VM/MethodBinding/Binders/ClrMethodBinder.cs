using Mint.Reflection;
using System;
using System.Collections.Generic;
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
        private static readonly MethodInfo INVALID_CONVERSION_METHOD =
            typeof(ClrMethodBinder).GetMethod(nameof(InvalidConversionMessage), BindingFlags.NonPublic | BindingFlags.Static);

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

            return methods.Concat(GetExtensionMethods(method)).Select(_ => new MethodInformation(_)).ToArray();
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
                select m
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
            methodInformations.Select(_ => _.ParameterInformation.Arity).Aggregate(new Arity(int.MaxValue, 0), Merge);

        private static Arity Merge(Arity r1, Arity r2)
        {
            var min = r1.Minimum;
            if(r2.Minimum < min) min = r2.Minimum;

            var max = r1.Maximum;
            if(r2.Maximum > max) max = r2.Maximum;

            return new Arity(min, max);
        }

        public override MethodBinder Alias(Symbol newName) => new ClrMethodBinder(newName, this);

        public override Expression Bind(CallInfo callInfo, Expression instance, Expression arguments)
        {
            // TODO assuming always ParameterKind.Required. change to accept Block, Rest, KeyRequired, KeyRest
            var length = callInfo.Parameters.Length;

            var filteredInfos = 
                methodInformations.Where( _ => _.ParameterInformation.Arity.Include(length) ).ToArray();

            if(filteredInfos.Length == 0)
            {
                return Throw(
                    New(
                        CTOR_ARGERROR,
                        Constant($"wrong number of arguments (given {length}, expected {Arity})")
                    ),
                    typeof(iObject)
                );
            }

            if(length == 0) // no parameters implies only 1 info
            {
                return CompileBody(methodInformations[0], instance);
            }

            var unsplatArgs = Enumerable.Range(0, length)
                .Select(i => (Expression) ArrayIndex(arguments, Constant(i)))
                .ToArray();
            var cases = filteredInfos.Select(_ => CreateSwitchCase(_, instance, unsplatArgs));

            //switch()
            //{
            //    case ...: { ... }
            //    default:
            //        throw new TypeError(InvalidConversionMessage(@filteredInfos, arguments));
            //}
            return Switch(
                typeof(iObject),
                Constant(true),
                Throw(New(
                    CTOR_TYPEERROR,
                    Call(INVALID_CONVERSION_METHOD, Constant(filteredInfos), arguments)
                ), typeof(iObject)),
                null,
                cases
            );
        }

        private Expression CompileBody(MethodInformation info, Expression instance, params Expression[] arguments)
        {
            if(!info.ParameterInformation.Arity.Include(arguments.Length))
            {
                return Throw(
                    New(
                        CTOR_ARGERROR,
                        Constant($"wrong number of arguments (given {arguments.Length}, expected {Arity})")
                    ),
                    typeof(iObject)
                );
            }

            if(info.MethodInfo.DeclaringType != null)
            {
                instance = Convert(instance, info.MethodInfo.DeclaringType);
            }

            var parameters = info.MethodInfo.GetParameters();

            if(info.MethodInfo.IsStatic)
            {
                var convertedArgs = arguments.Zip(parameters.Skip(1), ConvertArgument);
                arguments = new[] { instance }.Concat(convertedArgs).ToArray();
                instance = null;
            }
            else
            {
                arguments = arguments.Zip(parameters, ConvertArgument).ToArray();
            }

            return Box(Call(instance, info.MethodInfo, arguments));
        }

        private SwitchCase CreateSwitchCase(MethodInformation info, Expression instance, Expression[] arguments)
        {
            var parameters = info.MethodInfo.GetParameters().Select(_ => _.ParameterType).Select(_ => TYPES[_] ?? _);
            var condition = arguments.Zip(parameters, TypeIs).Cast<Expression>().Aggregate(AndAlso);
            var body = CompileBody(info, instance, arguments);
            return SwitchCase(body, condition);
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