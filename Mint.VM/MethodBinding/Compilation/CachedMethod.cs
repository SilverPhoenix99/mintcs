using Mint.MethodBinding.Methods;
using System;

namespace Mint.MethodBinding.Compilation
{
    internal abstract class CachedMethod
    {
        public long ClassId { get; }

        public Type InstanceType { get; }

        public MethodBinder Binder { get; }

        protected CachedMethod(long classId, Type instanceType, MethodBinder binder)
        {
            ClassId = classId;
            InstanceType = instanceType;
            Binder = binder;
        }
    }

    internal class CachedMethod<T> : CachedMethod
    {
        public T CachedCall { get; }

        public CachedMethod(long classId, Type instanceType, MethodBinder binder, T cachedCall)
            : base(classId, instanceType, binder)
        {
            CachedCall = cachedCall;
        }
    }
}
