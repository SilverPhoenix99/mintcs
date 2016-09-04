using Mint.MethodBinding.Methods;

namespace Mint.MethodBinding.Compilation
{
    internal class CachedMethod<T>
    {
        public long ClassId { get; }

        public MethodBinder Binder { get; }

        public T CachedCall { get; }

        public CachedMethod(long classId, MethodBinder binder, T cachedCall)
        {
            ClassId = classId;
            Binder = binder;
            CachedCall = cachedCall;
        }
    }
}
