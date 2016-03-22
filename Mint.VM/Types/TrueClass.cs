using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Mint
{
    public struct TrueClass : iObject
    {
        public long  Id                => 0x2;
        public Class Class             => CLASS;
        public Class SingletonClass    => CLASS;
        public Class CalculatedClass   => CLASS;
        public bool  HasSingletonClass => false;
        public bool  Frozen            => true;

        public void Freeze() { }

        public override string ToString() => "true";

        public string Inspect() => ToString();

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);

        public static implicit operator bool(TrueClass t) => true;

        #region Static

        public static readonly Class CLASS;

        static TrueClass()
        {
            CLASS = new Class(new Symbol(MethodBase.GetCurrentMethod().DeclaringType.Name), isSingleton: true);
            //Object.DefineClass(CLASS);
        }

        #endregion
    }
}
