using Mint.MethodBinding.Arguments;
using Mint.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Methods
{
    public sealed partial class ClrMethodBinder
    {
        /*
         * Stub:
         *
         * (iObject $instance, ArgumentBundle $bundle) => {
         *
         *     var $arguments = $bundle.Unbundle(@__MethodInformation.GetParameterBinders());
         *
         *     if(<type check all arguments>)
         *     {
         *         // e.g., when instance method and requires boxing:
         *         return Object.Box(<instance>.<method>($arguments[0], ..., $arguments[@n]));
         *
         *         // e.g., when static method and doesn't requires boxing:
         *         return <class>.<method>($instance, $arguments[0], ..., $arguments[@n]);
         *     }
         * }
         */
        private class CallEmitter
        {
            private MethodInfo Method { get; }
            private CallFrameBinder BundledFrame { get; }
            private LabelTarget Return { get; }
            private ParameterExpression ArgumentArray { get; }
            private ParameterInfo[] ParameterInfos { get; }

            public CallEmitter(MethodInfo method, CallFrameBinder bundledFrame, LabelTarget returnTarget)
            {
                Method = method;
                BundledFrame = bundledFrame;
                Return = returnTarget;
                ArgumentArray = Variable(typeof(iObject[]), "arguments");
                ParameterInfos = Method.GetParameters();
            }

            public Expression Bind()
            {
                if(BundledFrame.CallSite.Arity.Maximum == 0)
                {
                    return MakeCallWithReturn();
                }

                var unbundleExpression = ArgumentBundle.Expressions.CallUnbundle(BundledFrame.Arguments, Constant(Method));

                var argumentsAssign = Assign(ArgumentArray, unbundleExpression);

                var argumentTypeCheck = MakeArgumentTypeCheck();
                var callWithReturn = MakeCallWithReturn();
                var conditionalCall = IfThen(argumentTypeCheck, callWithReturn);

                return Block(new[] { ArgumentArray }, argumentsAssign, conditionalCall);
            }

            private Expression MakeArgumentTypeCheck() =>
                Enumerable.Range(0, BundledFrame.CallSite.ArgumentKinds.Count)
                .Select(MakeArgumentTypeCheck).Aggregate(AndAlso);

            private Expression MakeArgumentTypeCheck(int position)
            {
                var argument = ArrayIndex(ArgumentArray, Constant(position));
                var parameter = ParameterInfos[position];
                return TypeIs(argument, parameter.ParameterType);
            }

            private Expression MakeCallWithReturn()
            {
                var instance = BundledFrame.Instance;

                if(Method.DeclaringType != null)
                {
                    instance = instance.Cast(Method.DeclaringType);
                }

                var parameters = Method.GetParameters();
                IEnumerable<Expression> arguments;
                if(Method.IsStatic)
                {
                    var convertedArgs = parameters.Skip(1).Select(ConvertArgument);
                    arguments = new[] { instance }.Concat(convertedArgs);
                    instance = null;
                }
                else
                {
                    arguments = parameters.Select(ConvertArgument);
                }

                var callExpression = Box(Call(instance, Method, arguments));
                return Return(Return, callExpression);
            }

            private Expression ConvertArgument(ParameterInfo parameter)
            {
                var argument = ArrayIndex(ArgumentArray, Constant(parameter.Position));
                return TryConvert(argument, parameter.ParameterType);
            }
        }
    }
}