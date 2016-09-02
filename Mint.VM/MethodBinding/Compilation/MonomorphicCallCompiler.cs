using Mint.MethodBinding.Arguments;
using System;
using System.Linq.Expressions;

namespace Mint.MethodBinding.Compilation
{
    public class MonomorphicCallCompiler : BaseCallCompiler
    {
        public MonomorphicCallCompiler(CallSite callSite)
            : base(callSite)
        { }

        public override CallSite.Stub Compile() => DefaultCall;

        private iObject DefaultCall(iObject instance, ArgumentBundle bundle)
        {
            var binder = TryFindMethodBinder(instance);
            var instanceExpression = Expression.Constant(instance);
            var bundleExpression = Expression.Constant(bundle);

            var frame = new CallFrameBinder(CallSite, instanceExpression, bundleExpression);
            var body = binder.Bind(frame);
            var lambda = Expression.Lambda<Func<iObject>>(body).Compile();
            return lambda();
        }
    }
}
