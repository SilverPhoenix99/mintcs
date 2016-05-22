using System;
using System.Linq.Expressions;

namespace Mint.MethodBinding.CallCompilation
{
    public class MonomorphicCallCompiler : BaseCallCompiler
    {
        public MonomorphicCallCompiler(CallSite callSite)
            : base(callSite)
        { }

        public override Function Compile() => DefaultCall;

        private iObject DefaultCall(iObject instance, iObject[] arguments)
        {
            var binder = TryFindMethodBinder(instance);
            var instanceExpression = Expression.Constant(instance);
            var argumentsExpression = Expression.Constant(arguments);

            var invocationInfo = new InvocationInfo(CallSite.CallInfo, instanceExpression, argumentsExpression);
            var body = binder.Bind(invocationInfo);
            var lambda = Expression.Lambda<Func<iObject>>(body).Compile();
            return lambda();
        }
    }
}
