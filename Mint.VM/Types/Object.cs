namespace Mint
{
    public class Object : BaseObject
    {
        #region Static

        public static iObject Box(string obj) => new String(obj);
        public static iObject Box(short obj)  => new Fixnum(obj);
        public static iObject Box(int obj)    => new Fixnum(obj);
        public static iObject Box(long obj)   => new Fixnum(obj);
        public static iObject Box(float obj)  => new Float(obj);
        public static iObject Box(double obj) => new Float(obj);

        public static iObject Box(bool obj) => obj ? new TrueClass() : (iObject) new FalseClass();

        public static iObject Box(object value)
        {
            if(value is iObject) return (iObject) value;
            if(value is string) return Box((string) value);
            if(value is bool) return Box((bool) value);
            if(value is short) return Box((short) value);
            if(value is int) return Box((int) value);
            if(value is long) return Box((long) value);
            if(value is float) return Box((float) value);
            if(value is double) return Box((double) value);

            throw new ArgumentError(nameof(value));
        }

        public static bool ToBool(iObject obj) => obj != null && !(obj is NilClass) && !(obj is FalseClass);

        public static Module DefineModule(Module module)
        {
            if(module.Name.HasValue)
            {
                Class.OBJECT.Constants[module.Name.Value] = module;
            }
            return module;
        }

        internal static string MethodMissingInspect(iObject obj) => $"{obj.Inspect()}:{obj.Class.FullName}";

        internal static iObject Send(iObject obj, iObject name, params iObject[] args)
        {
            Symbol methodName;
            if(name is Symbol)
            {
                methodName = (Symbol) name;
            }
            else if(name is String)
            {
                methodName = new Symbol(((String) name).Value);
            }
            else
            {
                throw new TypeError($"{obj.Inspect()} is not a symbol nor a string");
            }
            
            var info = obj.GetType().GetMethod(methodName.Name);
            return (iObject) info.Invoke(obj, args);
        }

        #endregion
    }
}
