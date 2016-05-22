using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;

namespace Mint.MethodBinding.Binders
{
    internal class ClrMethodInvocationEmitter
    {
        private static readonly MethodInfo METHOD_GETPARAMETERBINDERS = Reflector<MethodInformation>.Method(
            _ => _.GetParameterBinders()
            );

        private MethodInformation Method { get; }
        private MethodInfo MethodInfo => Method.MethodInfo;
        private InvocationInfo BundleInfo { get; }
        private LabelTarget Return { get; }
        private ParameterExpression ArgumentArray { get; }
        private ParameterInfo[] ParameterInfos { get; }

        public ClrMethodInvocationEmitter(
            MethodInformation method,
            InvocationInfo bundleInfo,
            LabelTarget returnTarget
            )
        {
            Method = method;
            BundleInfo = bundleInfo;
            Return = returnTarget;
            ArgumentArray = Expression.Variable(typeof(iObject[]), "arguments");
            ParameterInfos = MethodInfo.GetParameters();
        }

        public Expression Bind()
        {
            if(BundleInfo.CallInfo.Arity == 0)
            {
                return MakeCallWithReturn();
            }

            var parameterBinders = Expression.Call(METHOD_GETPARAMETERBINDERS, Expression.Constant(Method));
            var unbundleExpression = ArgumentBundle.UnbundleCallExpression(
                BundleInfo.Arguments, parameterBinders);

            var argumentsAssign = Expression.Assign(ArgumentArray, unbundleExpression);

            var argumentTypeCheck = MakeArgumentTypeCheck();
            var callWithReturn = MakeCallWithReturn();
            var conditionalCall = Expression.IfThen(argumentTypeCheck, callWithReturn);

            return Expression.Block(new[] { ArgumentArray }, argumentsAssign, conditionalCall);
        }

        private Expression MakeArgumentTypeCheck() =>
            Enumerable.Range(0, BundleInfo.CallInfo.Arity)
            .Select(MakeArgumentTypeCheck).Aggregate(Expression.AndAlso);

        private Expression MakeArgumentTypeCheck(int position)
        {
            var argument = Expression.ArrayIndex(ArgumentArray, Expression.Constant(position));
            var parameter = ParameterInfos[position];
            return BaseMethodBinder.TypeIs(argument, parameter.ParameterType);
        }

        private Expression MakeCallWithReturn()
        {
            var instance = BundleInfo.Instance;

            if(MethodInfo.DeclaringType != null)
            {
                instance = Expression.Convert(instance, MethodInfo.DeclaringType);
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

            var callExpression = BaseMethodBinder.Box(Expression.Call(instance, MethodInfo, arguments));
            return Expression.Return(Return, callExpression);
        }

        private Expression ConvertArgument(ParameterInfo parameter)
        {
            var argument = Expression.ArrayIndex(ArgumentArray, Expression.Constant(parameter.Position));
            return BaseMethodBinder.TryConvert(argument, parameter.ParameterType);
        }
    }
}