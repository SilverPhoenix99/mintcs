namespace Mint.Binding
{
    public class CallSite
    {
        public CallSite(Binder binder)
        {
            Binder = binder;
            Call = Binder.Compile(this);
        }

        public CallSite(Symbol methodName)
        {
            Call = (instance, args) =>
            {
                Binder = new MegamorphicBinder(methodName);
                Call = Binder.Compile(this);
                return Call(instance, args);
            };
        }

        public Binder Binder { get; set; }
        public Method.Delegate Call { get; set; }
    }
}