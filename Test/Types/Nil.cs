﻿using System.Dynamic;
using System.Linq.Expressions;

namespace Mint
{
    public struct Nil : iObject
    {
        public static readonly Class CLASS;

        public long  Id                => 0x4;
        public Class Class             => CLASS;
        public Class SingletonClass    => CLASS;
        public bool  HasSingletonClass => false;
        public Class CalculatedClass   => CLASS;
        public bool  Frozen            => true;

        public void Freeze() {}

        public override string ToString() => "";

        public string Inspect() => "nil";

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public static implicit operator bool (Nil n) => false;

        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);

        public static bool IsNil(iObject other) => other == null || other is Nil;

        static Nil()
        {
            CLASS = new Class(new Symbol("NilClass"));
            //Object.DefineClass(CLASS);
        }
    }
}
