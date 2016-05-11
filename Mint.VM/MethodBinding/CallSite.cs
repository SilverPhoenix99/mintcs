using Mint.MethodBinding.CallCompilation;

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

        public override string ToString() => $"CallSite<{CallInfo}>";
    }
}
