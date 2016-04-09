﻿using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Mint
{
    public struct FalseClass : iObject
    {
        public long  Id                => 0x0;
        public Class Class             => CLASS;
        public Class SingletonClass    => CLASS;
        public Class CalculatedClass   => CLASS;
        public bool  HasSingletonClass => false;
        public bool  Frozen            => true;

        public void Freeze() { }

        public override string ToString() => "false";

        public string Inspect() => ToString();

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public iObject Send(iObject name, params iObject[] args) => Object.Send(this, name, args);

        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);

        public static implicit operator bool(FalseClass f) => false;

        #region Static

        public static readonly Class CLASS;

        static FalseClass()
        {
            CLASS = new Class(new Symbol(MethodBase.GetCurrentMethod().DeclaringType.Name), isSingleton: true);
            //Object.DefineClass(CLASS);

            CLASS.DefineMethod("to_s", Reflector<FalseClass>.Method(_ => _.ToString()));
            CLASS.DefineMethod("inspect", Reflector<FalseClass>.Method(_ => _.Inspect()));
        }

        #endregion
    }
}
