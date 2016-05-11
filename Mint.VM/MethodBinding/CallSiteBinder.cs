namespace Mint.MethodBinding
{
    public interface CallSiteBinder
    {
        CallSite CallSite { get; }

        Function Compile();
    }
}