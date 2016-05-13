using System;
using System.Linq.Expressions;

namespace Mint.MethodBinding.CallCompilation
{
    public class MonomorphicCallCompiler : CallCompiler
    {
        public CallSite CallSite { get; }

        public MonomorphicCallCompiler(CallSite callSite)
        {
            CallSite = callSite;
        }

        public Function Compile() => DefaultCall;

        private iObject DefaultCall(iObject instance, iObject[] arguments)
        {
            var methodBinder = instance.CalculatedClass.FindMethod(CallSite.CallInfo.MethodName);
            var instanceExpression = Expression.Constant(instance);
            var argumentsExpression = Expression.Constant(arguments);
            var body = methodBinder.Bind(CallSite.CallInfo, instanceExpression, argumentsExpression);
            var lambda = Expression.Lambda<Func<iObject>>(body).Compile();
            return lambda();
        }
    }
}
