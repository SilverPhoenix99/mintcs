namespace Mint.Binding
{
    public interface Binder
    {
        Method.Delegate Compile(CallSite site);
    }
}