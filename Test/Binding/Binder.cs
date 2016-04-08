using System;
using Mint;

namespace Mint.Binding
{
    interface Binder
    {
        Func<iObject, iObject[], iObject> Compile(CallSite site);
    }
}