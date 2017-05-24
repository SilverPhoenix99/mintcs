using System;
using System.Linq.Expressions;

namespace Mint.MethodBinding
{
    public class CallFrameBinder
    {
        public CallFrameBinder(CallSite callSite, Type instanceType, Expression instance, Expression arguments)
        {
            CallSite = callSite;
            InstanceType = instanceType;
            Instance = instance;
            Arguments = arguments;
        }


        public CallSite CallSite { get; }
        public Type InstanceType { get; }
        public Expression Instance { get; }
        public Expression Arguments { get; }
    }
}
