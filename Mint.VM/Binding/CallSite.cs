namespace Mint.Binding
{
    public class CallSite
    {
        public CallSite(Binder binder)
        {
            Binder = binder;
            Call = Binder.Compile(this);
        }

        public Binder Binder { get; set; }
        public Method.Delegate Call { get; set; }
    }
}