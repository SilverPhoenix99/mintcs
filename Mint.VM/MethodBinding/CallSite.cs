using Mint.MethodBinding.Arguments;
using Mint.MethodBinding.Compilation;
using System.Collections.Generic;

namespace Mint.MethodBinding
{
    public delegate iObject Function(iObject instance, params iObject[] arguments);

    public sealed class CallSite
    {
        public CallInfo CallInfo { get; }
        public CallCompiler CallCompiler { get; set; }
        public Function Call { get; set; }

        public CallSite(CallInfo callInfo)
        {
            CallInfo = callInfo;
            Call = DefaultCall;
        }

        private iObject DefaultCall(iObject instance, iObject[] arguments)
        {
            if(CallCompiler == null)
            {
                CallCompiler = new PolymorphicCallCompiler(this);
            }
            Call = CallCompiler.Compile();
            return Call(instance, arguments);
        }

        public static CallSite Create(Symbol methodName, Visibility visibility, IEnumerable<ArgumentKind> arguments) =>
            new CallSite(new CallInfo(methodName, visibility, arguments));

        public static CallSite Create(Symbol methodName, Visibility visibility, params ArgumentKind[] arguments) =>
            Create(methodName, visibility, (IEnumerable<ArgumentKind>) arguments);

        public static CallSite Create(Symbol methodName, params ArgumentKind[] arguments) =>
            Create(methodName, Visibility.Public, arguments);

        public override string ToString() => $"CallSite<{CallInfo}>";
    }
}
