using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding
{
    public sealed class ClrMethodBinder : BaseMethodBinder
    {
        private readonly Info[] Infos;

        public ClrMethodBinder(Symbol name, Module owner, MethodInfo method)
            : base(name, owner)
        {
            Infos = GetOverloads(method);
            Arity = CalculateArity();
        }

        private ClrMethodBinder(ClrMethodBinder other, bool copyValidation)
            : base(other, copyValidation)
        {
            Infos = (MethodInfo[]) other.Infos.Clone();
        }

        public override Expression Bind(CallSite site, Expression instance, params Expression[] args)
        {
            //if(infos.Length == 1)
            //{
            //    return SimpleBind(site, Infos[0], instance, args);
            //}

            var infos = Infos.Where( _ => _.Arity.Include(args.Length) ).ToArray();

            if(infos.Length == 0)
            {
                return Throw(
                    New(
                        CTOR_ARGERROR,
                        Constant($"none of the method overloads accepts {args.Length} arguments")
                    ),
                    typeof(iObject)
                );
            }

            var cases = infos.Select(_ => CreateSwitchCase(_, site, instance, args));

            throw new NotImplementedException();
        }

        private SwitchCase CreateSwitchCase(CallSite site, Info info, Expression instance, Expression[] args)
        {
            var parms = info.Method.GetParameters().Select(_ => _.ParameterType).ToArray();

            if(info.Method.IsStatic)
            {
                args     = new[] { instance }.Concat(args).ToArray();
                instance = null;
            }

            Expression body;
            if(parms.Length != args.Length)
            {
                var numArgs = args.Length - (info.Method.IsStatic ? 1 : 0);
                body = Throw(
                    New(
                        CTOR_ARGERROR,
                        Constant($"wrong number of arguments (given {numArgs}, expected {ArityString()})")
                    ),
                    typeof(iObject)
                );
            }
            else
            {

            }

            throw new NotImplementedException();
        }

        private Expression SimpleBind(CallSite site, Info info, Expression instance, Expression[] args)
        {
            var parms = info.Method.GetParameters().Select(_ => _.ParameterType).ToArray();
            if(info.Method.IsStatic)
            {
                args     = new[] { instance }.Concat(args).ToArray();
                instance = null;
            }

            if(parms.Length != args.Length)
            {
                var numArgs = args.Length - (info.Method.IsStatic ? 1 : 0);
                return Throw(
                    New(
                        CTOR_ARGERROR,
                        Constant($"wrong number of arguments (given {numArgs}, expected {ArityString()})")
                    ),
                    typeof(iObject)
                );
            }

            if(instance != null && info.Method.DeclaringType != null)
            {
                instance = Convert(instance, info.Method.DeclaringType);
            }

            Expression call = Call(instance, info.Method, args.Zip(parms, Convert));

            if(!typeof(iObject).IsAssignableFrom(info.Method.ReturnType))
            {
                call = Call(
                    OBJECT_BOX_METHOD,
                    Convert(call, typeof(object))
                );
            }

            return call;
        }

        public override MethodBinder Duplicate(bool copyValidation) => new ClrMethodBinder(this, copyValidation);

        private Range CalculateArity() => Infos.Select(_ => _.Arity).Aggregate(new Range(long.MaxValue, 0), Merge);

        private string ArityString()
        {
            long min = (Fixnum) Arity.Begin;
            long max = (Fixnum) Arity.End;

            return min == max ? min.ToString()
                 : max == long.MaxValue ? $"{min}+"
                 : Arity.ToString();
        }

        private static MethodInfo[] GetOverloads(MethodInfo method)
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

            return methods.Concat(GetExtensionMethods(method)).ToArray();
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

        internal static readonly MethodInfo OBJECT_BOX_METHOD = new Func<object, iObject>(Object.Box).Method;

        internal static readonly ConstructorInfo CTOR_ARGERROR = Reflector.Ctor<ArgumentError>(typeof(string));

        private class Info
        {
            // parameters, if specified, will follow this order:
            // Required, Optional, Rest, Required, (KeyRequired | KeyOptional), KeyRest, Block

            public readonly MethodInfo Method;
            public readonly Range Arity;

            public Info(MethodInfo method)
            {
                Method = method;
                Arity = CalculateArity();
            }

            private static Range CalculateArity()
            {
                var parameters = Method.GetParameters();
                var min = parameters.Count(_ => !_.IsOptional);
                var max = parameters.Length;

                if(Method.IsStatic)
                {
                    min--;
                    max--;
                }

                return new Range(min, max);
            }
        }
    }
}