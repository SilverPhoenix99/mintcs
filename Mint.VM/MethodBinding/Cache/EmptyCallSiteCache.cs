namespace Mint.MethodBinding.Cache
{
    public class EmptyCallSiteCache : BaseCallSiteCache
    {
        public EmptyCallSiteCache(CallSite callSite)
            : base(callSite)
        { }

        public override iObject Call()
        {
            var classId = CallFrame.Current.Instance.EffectiveClass.Id;

            UpgradeSiteCache(classId);
            return CallSite.CallCache.Call();
        }

        private void UpgradeSiteCache(long classId)
            => CallSite.CallCache = new MonomorphicCallSiteCache(CallSite, classId, TryFindMethodBinder());
    }
}