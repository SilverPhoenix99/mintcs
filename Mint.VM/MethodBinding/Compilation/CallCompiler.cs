namespace Mint.MethodBinding.Compilation
{
    public interface CallCompiler
    {
        CallSite.Function Compile();
    }
}