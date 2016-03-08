using System;
using System.Collections.Generic;

namespace Mint.Types
{
    public partial class Object : aObject
    {
        public static readonly Class BASIC_OBJECT_CLASS = new Class(name: "BasicObject") { Super = null };

        public static readonly Class CLASS = new Class(BASIC_OBJECT_CLASS, "Object");

        private Class realClass;
        protected IDictionary<string, iObject> variables = new Dictionary<string, iObject>();
        

        public Object(Class klass = null) : base()
        {
            realClass = klass ?? CLASS;
        }
        

        public override Class Class             => realClass.IsSingleton ? realClass.Super : realClass;
        public override Class RealClass         => realClass;
        public override bool  HasSingletonClass => realClass.IsSingleton;
        public override bool  Frozen            { get; protected set; }


        public override Class SingletonClass
        {
            get
            {
                return !realClass.IsSingleton
                    ? realClass = new Class(realClass) { IsSingleton = true }
                    : realClass;
            }
        }

        
        public override void Freeze()
        {
            Frozen = true;
        }


        public static iObject Convert(string s) => new String(s);


        public static iObject Convert(object o)
        {
            if(o is string) return Convert((string) o);

            return (iObject) o;
        }


        public static void DefineClass(Class klass)
        {
            CLASS.Constants[klass.Name] = klass;
        }


        static Object()
        {
            DefineClass(CLASS);
            DefineClass(BASIC_OBJECT_CLASS);

            // difficult cyclical dependency:
            if(Class.CLASS != null)
            {
                DefineClass(Class.CLASS);
            }

            BASIC_OBJECT_CLASS.Def("!", (Func<iObject, bool>) (
                (self) => !(self is False || self is Nil)
            ));

            BASIC_OBJECT_CLASS.Def("==", (Func<iObject, iObject, bool>) (
                (self, o) => self == o
            ));

            BASIC_OBJECT_CLASS.Def("!=", (Func<iObject, iObject, bool>) (
                (self, o) => self != o
            ));

            BASIC_OBJECT_CLASS.Def("__id__", (Func<iObject, Fixnum>) (
                (self) => self.Id
            ));

            BASIC_OBJECT_CLASS.Def("equal?", (Func<iObject, iObject, bool>) (
                (self, o) => self == o
            ));

            //__send__
            //instance_eval
            //instance_exec
        }
    }
}
