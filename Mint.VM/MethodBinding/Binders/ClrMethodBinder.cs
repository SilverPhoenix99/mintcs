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

        private Range CalculateArity() => methodInformations.Select(_ => _.Arity).Aggregate(new Range(long.MaxValue, 0), Merge);

        private static Range Merge(Range r1, Range r2)
        {
            long min = (Fixnum) r1.Begin;
            long val = (Fixnum) r2.Begin;
            if(val < min) min = val;

            long max = (Fixnum) r1.End;
            val = (Fixnum) r2.End;
            if(val > max) max = val;

            return new Range(min, max);
        }

        public override MethodBinder Alias(Symbol newName) => new ClrMethodBinder(newName, this);

        public override Expression Bind(CallInfo callInfo, Expression instance, Expression arguments)
        {
            // TODO assuming always ParameterKind.Required. change to accept Block, Rest, KeyRequired, KeyRest
            var length = callInfo.Parameters.Length;

            var filteredInfos = methodInformations.Where( _ => _.Arity.Include((Fixnum) length) ).ToArray();

            if(filteredInfos.Length == 0)
            {
                return Throw(
                    New(
                        CTOR_ARGERROR,
                        Constant($"wrong number of arguments (given {length}, expected {ArityString()})")
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

        private Expression CompileBody(MethodInformation info, Expression instance, params Expression[] args)
        {
            // site will be needed for non Required parameters

            if(!info.Arity.Include((Fixnum) args.Length))
            {
                return Throw(
                    New(
                        CTOR_ARGERROR,
                        Constant($"wrong number of arguments (given {args.Length}, expected {ArityString()})")
                    ),
                    typeof(iObject)
                );
            }

            var parameters = info.MethodInfo.GetParameters();

            if(info.MethodInfo.DeclaringType != null)
            {
                instance = Convert(instance, info.MethodInfo.DeclaringType);
            }

            if(info.MethodInfo.IsStatic)
            {
                var convertedArgs = args.Zip(parameters.Skip(1), ConvertArgument);
                args = new[] { instance }.Concat(convertedArgs).ToArray();
                instance = null;
            }
            else
            {
                args = args.Zip(parameters, ConvertArgument).ToArray();
            }

            return Box(Call(instance, info.MethodInfo, args));
        }

        private SwitchCase CreateSwitchCase(MethodInformation info, Expression instance, Expression[] args)
        {
            var parameters = info.MethodInfo.GetParameters().Select(_ => _.ParameterType).Select(_ => {
                Type type;
                return TYPES.TryGetValue(_, out type) ? type : _;
            });
            var condition  = args.Zip(parameters, TypeIs).Cast<Expression>().Aggregate(AndAlso);
            var body       = CompileBody(info, instance, args);
            return SwitchCase(body, condition);
        }

        private string ArityString()
        {
            long min = (Fixnum) Arity.Begin;
            long max = (Fixnum) Arity.End;

            return min == max ? min.ToString()
                 : max == long.MaxValue ? $"{min}+"
                 : Arity.ToString();
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