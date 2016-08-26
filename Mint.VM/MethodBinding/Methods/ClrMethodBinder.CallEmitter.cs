using System.CodeDom.Compiler;
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
        private class CallEmitter
        {
            private static readonly MethodInfo METHOD_GETPARAMETERBINDERS = Reflector<MethodInformation>.Method(
                _ => _.GetParameterBinders()
            );

            private MethodInformation Method { get; }
            private MethodInfo MethodInfo => Method.MethodInfo;
            private CallFrameBinder BundledFrame { get; }
            private LabelTarget Return { get; }
            private ParameterExpression ArgumentArray { get; }
            private ParameterInfo[] ParameterInfos { get; }

            public CallEmitter(MethodInformation method, CallFrameBinder bundledFrame, LabelTarget returnTarget)
            {
                Method = method;
                BundledFrame = bundledFrame;
                Return = returnTarget;
                ArgumentArray = Variable(typeof(iObject[]), "arguments");
                ParameterInfos = MethodInfo.GetParameters();
            }

            public Expression Bind()
            {
                if(BundledFrame.CallSite.Arity == 0)
                {
                    return MakeCallWithReturn();
                }

                var parameterBinders = Call(METHOD_GETPARAMETERBINDERS, Constant(Method));
                var unbundleExpression = ArgumentBundle.Expressions.CallUnbundle(BundledFrame.Arguments, parameterBinders);

                var argumentsAssign = Assign(ArgumentArray, unbundleExpression);

                var argumentTypeCheck = MakeArgumentTypeCheck();
                var callWithReturn = MakeCallWithReturn();
                var conditionalCall = IfThen(argumentTypeCheck, callWithReturn);

                return Block(new[] { ArgumentArray }, argumentsAssign, conditionalCall);
            }

            private Expression MakeArgumentTypeCheck() =>
                Enumerable.Range(0, BundledFrame.CallSite.Arity)
                .Select(MakeArgumentTypeCheck).Aggregate(AndAlso);

            private Expression MakeArgumentTypeCheck(int position)
            {
                var argument = ArrayIndex(ArgumentArray, Constant(position));
                var parameter = ParameterInfos[position];
                return BaseMethodBinder.TypeIs(argument, parameter.ParameterType);
            }

            private Expression MakeCallWithReturn()
            {
                var instance = BundledFrame.Instance;

                if(MethodInfo.DeclaringType != null)
                {
                    instance = instance.Cast(MethodInfo.DeclaringType);
                }

                var parameters = MethodInfo.GetParameters();
                IEnumerable<Expression> arguments;
                if(MethodInfo.IsStatic)
                {
                    var convertedArgs = parameters.Skip(1).Select(ConvertArgument);
                    arguments = new[] { instance }.Concat(convertedArgs);
                    instance = null;
                }
                else
                {
                    arguments = parameters.Select(ConvertArgument);
                }

                var callExpression = Box(Call(instance, MethodInfo, arguments));
                return Return(Return, callExpression);
            }

            private Expression ConvertArgument(ParameterInfo parameter)
            {
                var argument = ArrayIndex(ArgumentArray, Constant(parameter.Position));
                return BaseMethodBinder.TryConvert(argument, parameter.ParameterType);
            }

            private static Expression Box(Expression expression)
            {
                expression = BaseMethodBinder.Box(expression);
                var result = Variable(typeof(iObject), "result");
                return Block(
                    typeof(iObject),
                    new[] { result },
                    Assign(result, expression),
                    Condition(Equal(result, Constant(null)), BindingUtils.NIL, result)
                );
            }
        }
    }
}