using Mint.MethodBinding.Arguments;
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
         * global iObject $instance;
         * global ArgumentBundle $bundle;
         * global iObject[] $arguments;
         *     
         * case {
         *     $arguments = $bundle.Bind(@methodInfo);
         *     $arguments != null && $arguments[0] is <Type> && ...
         * }:
         * {
         *     // instance methods:
         *     return Object.Box($instance.<Method>((<cast>) $arguments[0], ...));
         * 
         *     // static methods:
         *     return Object.Box(<Type>.<Method>($instance, (<cast>) $arguments[0], ...));
         * }
         */
        private class CallEmitter
        {
            private MethodInfo Method { get; }

            private CallFrameBinder BundledFrame { get; }

            private ParameterExpression ArgumentArray { get; }

            private ParameterInfo[] ParameterInfos { get; }

            public CallEmitter(MethodInfo method, CallFrameBinder bundledFrame, ParameterExpression argumentsArray)
            {
                Method = method;
                BundledFrame = bundledFrame;
                ArgumentArray = argumentsArray;
                ParameterInfos = Method.GetParameters();
            }

            public SwitchCase Bind()
            {
                var condition = CreateCondition();
                var body = CreateBody();
                return SwitchCase(body, condition);
            }

            private Expression CreateCondition()
            {
                var bindExpression = ArgumentBundle.Expressions.Bind(BundledFrame.Arguments, Constant(Method));
                Expression argumentCheck = NotEqual(ArgumentArray, Constant(null));

                if(ParameterInfos.Length != 0)
                {
                    argumentCheck = AndAlso(argumentCheck, TypeCheckArgumentsExpression());
                }

                return Block(
                    Assign(ArgumentArray, bindExpression),
                    argumentCheck
                );
            }

            private Expression TypeCheckArgumentsExpression() =>
                Enumerable.Range(0, ParameterInfos.Length).Select(TypeCheckArgumentExpression).Aggregate(AndAlso);

            private Expression TypeCheckArgumentExpression(int position)
            {
                var argument = ArrayIndex(ArgumentArray, Constant(position));
                var parameter = ParameterInfos[position];
                return TypeIs(argument, parameter.ParameterType);
            }

            private Expression CreateBody()
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

                return Box(Call(instance, Method, arguments));
            }

            private Expression ConvertArgument(ParameterInfo parameter)
            {
                var argument = ArrayIndex(ArgumentArray, Constant(parameter.Position));
                return TryConvert(argument, parameter.ParameterType);
            }
        }
    }
}