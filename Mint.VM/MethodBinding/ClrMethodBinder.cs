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
        private readonly MethodInfo[] infos;

        public ClrMethodBinder(Symbol name, Module owner, MethodInfo method)
            : base(name, owner)
        {
            infos = GetOverloads(method);
            Arity = CalculateArity();
        }

        private ClrMethodBinder(ClrMethodBinder other, bool copyValidation)
            : base(other, copyValidation)
        {
            infos = (MethodInfo[]) other.infos.Clone();
        }

        public override Expression Bind(Expression instance, IEnumerable<Expression> args)
        {
            //TODO parameters

            //if(infos.Length == 1)
            {
                return SimpleBind(infos[0], instance, args);
            }

            // TODO : multiple binding
        }

        private Expression SimpleBind(MethodInfo method, Expression instance, IEnumerable<Expression> args)
        {
            var parms = method.GetParameters().Select(_ => _.ParameterType);
            if(method.IsStatic)
            {
                args     = new[] { instance }.Concat(args);
                instance = null;
            }

            if(instance != null && method.DeclaringType != null)
            {
                instance = Convert(instance, method.DeclaringType);
            }

            var argsArray  = args.ToArray();
            var parmsArray = parms.ToArray();
            var argsLength = argsArray.Length;

            if(method.IsStatic)
            {
                argsLength--;
            }

            if(parmsArray.Length != argsArray.Length)
            {
                return Throw(
                    New(
                        CTOR_ARGERROR,
                        Constant($"wrong number of arguments (given {argsLength}, expected {ArityString()})")
                    ),
                    typeof(iObject)
                );
            }

            args = argsArray.Zip(parmsArray, Convert);
            Expression call = Call(instance, method, args);

            if(!typeof(iObject).IsAssignableFrom(method.ReturnType))
            {
                call = Call(
                    OBJECT_BOX_METHOD,
                    Convert(call, typeof(object))
                );
            }
            

            return call;
        }

        public override MethodBinder Duplicate(bool copyValidation) => new ClrMethodBinder(this, copyValidation);

        private Range CalculateArity()
        {
            var r1 = new Range(long.MaxValue, 0);
            return infos.Select(CalculateArity).Aggregate(r1, Merge);
        }

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

        private static Range CalculateArity(MethodInfo info)
        {
            var infos = info.GetParameters();
            var min = infos.Count(_ => !_.IsOptional);
            var max = infos.Length;

            if(info.IsStatic)
            {
                min--;
                max--;
            }

            return new Range(min, max);
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
            // Req, Opt, Rest, Req, KeyReq | KeyOpt, KeyRest, Block

            public readonly MethodInfo Method;
            public readonly int  PrefixReq;
            public readonly int  Optional;
            public readonly bool Rest;
            public readonly int  PostfixReq;
            public readonly int  KeyReq;
            public readonly int  KeyOpt;
            public readonly bool KeyRest;
            public readonly bool Block;

            public Info(int prefixReq, int optional, bool rest, int postfixReq, int keyReq, int keyOpt, bool keyRest, bool block)
            {
                PrefixReq  = prefixReq;
                Optional   = optional;
                Rest       = rest;
                PostfixReq = postfixReq;
                KeyReq     = keyReq;
                KeyOpt     = keyOpt;
                KeyRest    = keyRest;
                Block      = block;
            }

            public Range Arity
            {
                get
                {
                    long min = PrefixReq + PostfixReq + KeyReq;
                    var max = Rest || KeyRest ? long.MaxValue : min + Optional + KeyOpt + (Block ? 1 : 0);
                    return new Range(min, max);
                }
            }
        }
    }
}