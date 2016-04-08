using System;
using Mint;

namespace Mint.Binding
{
    class CallSite
    {
        public CallSite(Binder binder)
        {
            Binder = binder;
            Call = Binder.Compile(this);
        }

        protected internal Binder Binder { get; set; }
        protected internal Func<iObject, iObject[], iObject> Call { get; set; }
    }
}