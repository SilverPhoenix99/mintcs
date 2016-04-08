using System;
using Mint;

namespace Mint.Binding
{
    interface Binder
    {
        CallSite.Function Compile(CallSite site);
    }
}