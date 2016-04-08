using System;
using Mint;

namespace Mint.Binding
{
    class CallSite
    {
        public delegate iObject Function(iObject instance, iObject[] args);

        public CallSite(Binder binder)
        {
            Binder = binder;
            Call = Binder.Compile(this);
        }

        protected internal Binder Binder { get; set; }
        protected internal Function Call { get; set; }
    }
}