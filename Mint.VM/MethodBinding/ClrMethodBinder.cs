using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding
{
    public sealed partial class ClrMethodBinder : BaseMethodBinder
    {
        private readonly Info[] infos;

        public ClrMethodBinder(Symbol name, Module owner, MethodInfo method)
            : base(name, owner)
        {
            infos = GetOverloads(method);
            Debug.Assert(infos.Length != 0);
            Arity = CalculateArity();
        }

        private ClrMethodBinder(ClrMethodBinder other, bool copyValidation)
            : base(other, copyValidation)
        {
            infos = (Info[]) other.infos.Clone();
        }

        public override MethodBinder Duplicate(bool copyValidation) => new ClrMethodBinder(this, copyValidation);

        public override Expression Bind(CallSite site, Expression instance, Expression args)
        {
            // TODO assuming always ParameterKind.Required. change to accept Block, Rest, KeyRequired, KeyRest
            var length = site.Parameters.Length;

            var filteredInfos = infos.Where( _ => _.Arity.Include((Fixnum) length) ).ToArray();

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

            if(length == 0) // no parameters => only 1 info
            {
                return CompileBody(infos[0], instance);
            }

            var unsplatArgs = Enumerable.Range(0, length)
                .Select(i => (Expression) ArrayIndex(args, Constant(i)))
                .ToArray();
            var cases = filteredInfos.Select(_ => CreateSwitchCase(_, instance, unsplatArgs));

            //switch()
            //{
            //    case ...: { ... }
            //    default:
            //        throw new TypeError(InvalidConversionMessage(@filteredInfos, args));
            //}
            return Switch(
                typeof(iObject),
                Constant(true),
                Throw(New(
                    CTOR_TYPEERROR,
                    Call(INVALID_CONVERSION_METHOD, Constant(filteredInfos), args)
                ), typeof(iObject)),
                null,
                cases
            );
        }

        private SwitchCase CreateSwitchCase(Info info, Expression instance, Expression[] args)
        {
            var parameters = info.Method.GetParameters().Select(_ => _.ParameterType).Select(_ => {
                Type type;
                return TYPES.TryGetValue(_, out type) ? type : _;
            });
            var condition  = args.Zip(parameters, TypeIs).Cast<Expression>().Aggregate(AndAlso);
            var body       = CompileBody(info, instance, args);
            return SwitchCase(body, condition);
        }

        private Expression CompileBody(Info info, Expression instance, params Expression[] args)
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

            var parameters = info.Method.GetParameters();

            if(info.Method.DeclaringType != null)
            {
                instance = Convert(instance, info.Method.DeclaringType);
            }

            if(info.Method.IsStatic)
            {
                var convertedArgs = args.Zip(parameters.Skip(1), ConvertArg);
                args = new[] { instance }.Concat(convertedArgs).ToArray();
                instance = null;
            }
            else
            {
                args = args.Zip(parameters, ConvertArg).ToArray();
            }

            return Call(instance, info.Method, args);
        }

        private Range CalculateArity() => infos.Select(_ => _.Arity).Aggregate(new Range(long.MaxValue, 0), Merge);

        private static Expression ConvertArg(Expression arg, ParameterInfo parameter)
        {
            Type type;
            if(TYPES.TryGetValue(parameter.ParameterType, out type))
            {
                arg = Convert(arg, type);
            }

            return Convert(arg, parameter.ParameterType);
        }

        private string ArityString()
        {
            long min = (Fixnum) Arity.Begin;
            long max = (Fixnum) Arity.End;

            return min == max ? min.ToString()
                 : max == long.MaxValue ? $"{min}+"
                 : Arity.ToString();
        }

        #region Static

        private static readonly Dictionary<Type, Type> TYPES = new Dictionary<Type, Type>(11)
        {
            { typeof(string),        typeof(String) },
            { typeof(StringBuilder), typeof(String) },
            { typeof(sbyte),         typeof(Fixnum) },
            { typeof(byte),          typeof(Fixnum) },
            { typeof(short),         typeof(Fixnum) },
            { typeof(ushort),        typeof(Fixnum) },
            { typeof(int),           typeof(Fixnum) },
            { typeof(uint),          typeof(Fixnum) },
            { typeof(long),          typeof(Fixnum) },
            { typeof(float),         typeof(Float)  },
            { typeof(double),        typeof(Float)  }
        };

        private static Info[] GetOverloads(MethodInfo method)
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

            return methods.Concat(GetExtensionMethods(method)).Select(_ => new Info(_)).ToArray();
        }

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
                select m;
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

        private static string InvalidConversionMessage(Info[] infos, iObject[] args)
        {
            // TODO

            //for(var i = 0; i < args.Length; i++)
            //{
            //    var arg = args[i];
            //    var types = infos.Select(_ => _.Method.GetParameters()[i]).an;
            //}

            //msg = "argument {index}: no implicit conversion of {type} to {string.Join(" or ", types)}";
            return "no implicit conversion exists";
        }

        internal static readonly MethodInfo OBJECT_BOX_METHOD = new Func<object, iObject>(Object.Box).Method;

        private static readonly MethodInfo INVALID_CONVERSION_METHOD =
            typeof(ClrMethodBinder).GetMethod(nameof(InvalidConversionMessage), BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly ConstructorInfo CTOR_ARGERROR  = Reflector.Ctor<ArgumentError>(typeof(string));
        private static readonly ConstructorInfo CTOR_TYPEERROR = Reflector.Ctor<TypeError>(typeof(string));

        #endregion
    }
}