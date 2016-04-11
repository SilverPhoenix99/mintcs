using System;

namespace Mint.MethodBinding
{
    public delegate iObject Function(iObject instance, iObject[] args);

    public class CallSite
    {
        public CallSite(Symbol methodName, Range arity, CallSiteBinder binder = null)
        {
            MethodName = methodName;
            Arity = arity;
            Binder = binder;
            Call = Binder != null ? Binder.Compile(this) : DefaultCall;
        }

        public Symbol         MethodName { get; }
        public Range          Arity      { get; }
        public CallSiteBinder Binder     { get; set; }
        public Function       Call       { get; set; }

        private iObject DefaultCall(iObject instance, iObject[] args)
        {
            Binder = new MegamorphicSiteBinder(); // TODO change default to PolymorphicBinder
            Call = Binder.Compile(this);
            return Call(instance, args);
        }
    }
}