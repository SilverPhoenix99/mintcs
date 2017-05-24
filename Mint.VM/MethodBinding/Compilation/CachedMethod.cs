using Mint.MethodBinding.Methods;
using System;

namespace Mint.MethodBinding.Compilation
{
    internal abstract class CachedMethod
    {
        protected CachedMethod(long classId, Type instanceType, MethodBinder binder)
        {
            ClassId = classId;
            InstanceType = instanceType;
            Binder = binder;
        }
    

        public long ClassId { get; }
        public Type InstanceType { get; }
        public MethodBinder Binder { get; }
    }


    internal class CachedMethod<T> : CachedMethod
    {
        public CachedMethod(long classId, Type instanceType, MethodBinder binder, T cachedCall)
            : base(classId, instanceType, binder)
        {
            CachedCall = cachedCall;
        }
    

        public T CachedCall { get; }
    }
}
