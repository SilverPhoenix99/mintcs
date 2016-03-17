using System;
using System.Collections.Generic;

namespace Mint
{
    public partial class Object : aObject
    {
        private Class calculatedClass;
        protected IDictionary<Symbol, iObject> variables = new Dictionary<Symbol, iObject>();
        
        public Object(Class klass = null) : base()
        {
            calculatedClass = klass ?? CLASS;
        }

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
        
        public override void Freeze()
        {
            Frozen = true;
        }
        
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

        #region Static

        public static readonly Class BASIC_OBJECT_CLASS;

        public static readonly Class CLASS;

        public static iObject Box(string obj) => new String(obj);

        public static iObject Box(bool obj) => obj ? new True() : (iObject) new False();
        
        public static iObject Box(object o)
        {
            if(o is string) return Box((string) o);
            if(o is bool)   return Box((bool) o);

            return (iObject) o;
        }
        
        public static void DefineClass(Class klass)
        {
            if(klass.Name.HasValue)
            {
                CLASS.Constants[klass.Name.Value] = klass;
            }
        }
        
        static Object()
        {
            BASIC_OBJECT_CLASS = new Class(null, new Symbol("BasicObject"));
            CLASS = new Class(BASIC_OBJECT_CLASS, new Symbol("Object"));
        
            /*DefineClass(CLASS);
            DefineClass(BASIC_OBJECT_CLASS);

            // difficult cyclical dependency:
            if(Class.CLASS != null)
            {
                DefineClass(Class.CLASS);
            }

            BASIC_OBJECT_CLASS.DefineMethod(new Symbol("!"), (Func<iObject, bool>) (
                (self) => !(self is False || self is Nil)
            ));

            BASIC_OBJECT_CLASS.DefineMethod(new Symbol("=="), (Func<iObject, iObject, bool>) (
                (self, o) => self == o
            ));

            BASIC_OBJECT_CLASS.DefineMethod(new Symbol("!="), (Func<iObject, iObject, bool>) (
                (self, o) => self != o
            ));

            BASIC_OBJECT_CLASS.DefineMethod(new Symbol("__id__"), (Func<iObject, Fixnum>) (
                (self) => self.Id
            ));

            BASIC_OBJECT_CLASS.DefineMethod(new Symbol("equal?"), (Func<iObject, iObject, bool>) (
                (self, o) => self == o
            ));*/

            //__send__
            //instance_eval
            //instance_exec
        }

        #endregion
    }
}
