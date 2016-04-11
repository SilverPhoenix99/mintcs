namespace Mint.MethodBinding
{
    public interface CallSiteBinder
    {
        Function Compile(CallSite site);
    }
}