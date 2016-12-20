using System;
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
        private interface CallEmitter
        {
            SwitchCase Bind();
        }

        /*
         * Generated Stub:
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
         *     return Object.Box($instance.<Method>((<cast>) $arguments[0], ...));
         * }
         */
        private class InstanceCallEmitter : CallEmitter
        {
            protected MethodInfo Method { get; }

            private CallFrameBinder BundledFrame { get; }

            private ParameterExpression ArgumentArray { get; }

            protected ParameterInfo[] ParameterInfos { get; }

            protected virtual int Offset => 0;

            public InstanceCallEmitter(MethodInfo method, CallFrameBinder bundledFrame, ParameterExpression argumentsArray)
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
                var bindExpression = ArgumentBundle.Expressions.TryBind(BundledFrame.Arguments, Constant(Method));
                Expression argumentCheck = NotEqual(ArgumentArray, Constant(null));

                if(ParameterInfos.Length > Offset)
                {
                    argumentCheck = AndAlso(argumentCheck, TypeCheckArgumentsExpression());
                }

                return Block(
                    Assign(ArgumentArray, bindExpression),
                    argumentCheck
                );
            }

            private Expression TypeCheckArgumentsExpression()
            {
                return Enumerable.Range(Offset, ParameterInfos.Length - Offset)
                    .Select(TypeCheckArgumentExpression)
                    .Aggregate(AndAlso);
            }

            private Expression TypeCheckArgumentExpression(int position)
            {
                var argument = ArrayIndex(ArgumentArray, Constant(position));
                var parameter = ParameterInfos[position];
                return TypeIs(argument, parameter.ParameterType);
            }

            private Expression CreateBody() => Box(Call(GetInstance(), Method, GetArguments()));

            protected virtual Expression GetInstance() => GetConvertedInstance();

            protected Expression GetConvertedInstance()
            {
                var type = TryGetInstanceType();
                return type == null ? BundledFrame.Instance : BundledFrame.Instance.Cast(type);
            }

            protected virtual IEnumerable<Expression> GetArguments() => ParameterInfos.Select(ConvertArgument);

            protected virtual Type TryGetInstanceType() => Method.DeclaringType != null ? Method.DeclaringType : null;

            protected Expression ConvertArgument(ParameterInfo parameter)
            {
                var argument = ArrayIndex(ArgumentArray, Constant(parameter.Position));
                return TryConvert(argument, parameter.ParameterType);
            }
        }
    }
}