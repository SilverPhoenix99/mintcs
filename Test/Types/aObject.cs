using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Threading;

namespace Mint
{
    public abstract class aObject : aFrozenObject
    {
        private Class calculatedClass;
        protected IDictionary<Symbol, iObject> variables = new Dictionary<Symbol, iObject>();

        public aObject(Class klass) : base()
        {
            calculatedClass = klass;
        }

        public aObject() : this(Object.CLASS) { }

        public override Class Class             => HasSingletonClass ? calculatedClass.Superclass : calculatedClass;
        public override Class CalculatedClass   => calculatedClass;
        public override bool  HasSingletonClass => calculatedClass.IsSingleton;
        public override bool  Frozen            { get; protected set; }

        public override Class SingletonClass
        {
            get
            {
                return !calculatedClass.IsSingleton
                    ? calculatedClass = new Class(calculatedClass, isSingleton: true)
                    : calculatedClass;
            }
        }

        public override void Freeze() { Frozen = true; }

        public virtual Method FindMethod(Symbol name)
        {
            // Method resolution:
            //   1. Methods defined in the object's singleton class (i.e. the object itself)
            //   2. Modules mixed into the singleton class in reverse order of inclusion
            //   3. Methods defined by the object's class
            //   4. Modules included into the object's class in reverse order of inclusion
            //   5. Methods defined by the object's superclass.

            return CalculatedClass.FindMethod(name);
        }
    }
}
