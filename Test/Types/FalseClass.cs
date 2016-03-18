using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Mint
{
    public struct FalseClass : iObject
    {
        public long  Id                => 0x0;
        public Class Class             => CLASS;
        public Class SingletonClass    => CLASS;
        public bool  HasSingletonClass => false;
        public Class CalculatedClass   => CLASS;
        public bool  Frozen            => true;

        public void Freeze() { }
        
        public override string ToString() => "false";

        public string Inspect() => ToString();

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);

        #region Static

        public static readonly Class CLASS;

        public static implicit operator bool(FalseClass f) => false;

        static FalseClass()
        {
            CLASS = new Class(new Symbol(MethodBase.GetCurrentMethod().DeclaringType.Name));
            //Object.DefineClass(CLASS);
        }

        #endregion
    }
}
