using Mint.MethodBinding.Arguments;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;
using Mint.Reflection;

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
         *     return Object.Box(((<cast>) $instance).<Method>((<cast>) $arguments[0], ...));
         * }
         */
        private class InstanceCallEmitter : CallEmitter
        {
            protected MethodMetadata Method { get; }

            private CallFrameBinder BundledFrame { get; }

            private ParameterExpression ArgumentArray { get; }

            public InstanceCallEmitter(MethodMetadata method,
                                       CallFrameBinder bundledFrame,
                                       ParameterExpression argumentsArray)
            {
                Method = method;
                BundledFrame = bundledFrame;
                ArgumentArray = argumentsArray;
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

                if(Method.Parameters.Count > 0)
                {
                    argumentCheck = AndAlso(argumentCheck, TypeCheckArgumentsExpression());
                }

                return Block(
                    Assign(ArgumentArray, bindExpression),
                    argumentCheck
                );
            }

            private Expression TypeCheckArgumentsExpression() =>
                Method.Parameters.Select(TypeCheckArgumentExpression).Aggregate(AndAlso);

            private Expression TypeCheckArgumentExpression(ParameterMetadata parameter) =>
                TypeIs(GetArgument(parameter), parameter.Parameter.ParameterType);

            private Expression CreateBody() => Box(Call(GetInstance(), Method.Method, GetArguments()));

            protected virtual Expression GetInstance() => GetConvertedInstance();

            protected Expression GetConvertedInstance()
            {
                var type = Method.Method.DeclaringType;
                return type == null ? BundledFrame.Instance : BundledFrame.Instance.Cast(type);
            }

            protected virtual IEnumerable<Expression> GetArguments() => Method.Parameters.Select(ConvertArgument);

            protected Expression ConvertArgument(ParameterMetadata parameter) =>
                TryConvert(GetArgument(parameter), parameter.Parameter.ParameterType);

            protected BinaryExpression GetArgument(ParameterMetadata parameter)
            {
                var position = GetArgumentPosition(parameter);
                return ArrayIndex(ArgumentArray, Constant(position));
            }

            protected int GetArgumentPosition(ParameterMetadata parameter) =>
                parameter.Position - Method.ParameterOffset;
        }
    }
}