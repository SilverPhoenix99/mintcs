using Mint.MethodBinding;
using System;

namespace Mint
{
    public partial class Class
    {
        public static readonly Class BASIC_OBJECT;
        public static readonly Class OBJECT;
        public static readonly Class MODULE;
        public static readonly Class CLASS;
        public static readonly Class NUMERIC;
        public static readonly Class FLOAT;
        public static readonly Class COMPLEX;
        public static readonly Class RATIONAL;
        public static readonly Class INTEGER;
        public static readonly Class FIXNUM;
        public static readonly Class NIL;
        public static readonly Class FALSE;
        public static readonly Class TRUE;
        public static readonly Class ARRAY;
        public static readonly Class HASH;
        public static readonly Class RANGE;
        public static readonly Class REGEXP;
        public static readonly Class STRING;
        public static readonly Class SYMBOL;

        public static readonly Module KERNEL;
        
        private static readonly CallSite Eq;

        static Class()
        {
            // ==       |   iObject#Equals(iObject) (by default equal?)
            // equal?   |   object::ReferenceEquals(object, object)
            // eql?     |   (iObject i, iObject a) => i.hash == a.hash

            BASIC_OBJECT = ClassBuilder<Object>.Describe(null, "BasicObject")
                .AttrReader("__id__", () => ((iObject) null).Id )
                .DefMethod( "==",     () => ((iObject) null).Equals(default(object)) )
                .DefLambda( "!",      (Func<iObject, bool>) (_ => !Object.ToBool(_)) )
                .DefMethod( "equal?", () => ((iObject) null).Equal(default(object)) )

                .DefLambda("!=", (Func<iObject, iObject, bool>) ( (l, r) => !Object.ToBool(Class.Eq.Call(l, r)) ) )

                //.DefMethod("__send__",                   () => ??? );
                //.DefMethod("instance_eval",              () => ??? );
                //.DefMethod("instance_exec",              () => ??? );
                //.DefMethod("__binding__",                () => ??? );
                //.DefMethod("initialize",                 () => ??? );
                //.DefMethod("method_missing",             () => ??? );
                //.DefMethod("singleton_method_added",     () => ??? );
                //.DefMethod("singleton_method_removed",   () => ??? );
                //.DefMethod("singleton_method_undefined", () => ??? );
            ;

            BASIC_OBJECT.Constants[BASIC_OBJECT.Name.Value] = BASIC_OBJECT;

            // TODO define in Kernel module
            OBJECT = ClassBuilder<Object>.Describe(BASIC_OBJECT);

            MODULE = ClassBuilder<Module>.Describe()
                .DefMethod("to_s",    _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            CLASS = ClassBuilder<Class>.Describe(MODULE);

            // required hack
            CLASS.calculatedClass = CLASS;

            KERNEL = ModuleBuilder<iObject>.Describe("Kernel")
                .AttrReader("class",   _ => _.Class )
                .DefMethod( "to_s",    () => ((FrozenObject) null).ToString() )
                .DefMethod( "inspect", () => ((FrozenObject) null).Inspect() )
            ;

            OBJECT.Include(KERNEL);
            Object.DefineModule(KERNEL);

            NUMERIC = new Class(new Symbol("Numeric"));

            FLOAT = ClassBuilder<Float>.Describe(NUMERIC)
                .DefMethod("to_s",    _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            COMPLEX = ClassBuilder<Complex>.Describe(NUMERIC);

            RATIONAL = ClassBuilder<Rational>.Describe(NUMERIC);

            INTEGER = new Class(NUMERIC, new Symbol("Integer"));

            FIXNUM = ClassBuilder<Fixnum>.Describe(INTEGER)
                .DefMethod("to_s",    _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            NIL = ClassBuilder<NilClass>.Describe()
                .DefMethod("to_s",    _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            FALSE = ClassBuilder<FalseClass>.Describe()
                .DefMethod("to_s",    _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            TRUE = ClassBuilder<TrueClass>.Describe()
                .DefMethod("to_s",    _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            ARRAY = ClassBuilder<Array>.Describe()
                .DefMethod("to_s",    _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            HASH = ClassBuilder<Hash>.Describe()
                .DefMethod("to_s",    _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            RANGE = ClassBuilder<Range>.Describe()
                .DefMethod("to_s",    _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            REGEXP = ClassBuilder<Regexp>.Describe();

            STRING = ClassBuilder<String>.Describe()
                .DefMethod("to_s",    _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            SYMBOL = ClassBuilder<Symbol>.Describe()
                .DefMethod("to_s",    _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            Object.DefineModule(BASIC_OBJECT);
            Object.DefineModule(OBJECT);
            Object.DefineModule(MODULE);
            Object.DefineModule(CLASS);
            Object.DefineModule(ARRAY);
            Object.DefineModule(COMPLEX);
            Object.DefineModule(FALSE);
            Object.DefineModule(NUMERIC);
            Object.DefineModule(INTEGER);
            Object.DefineModule(FIXNUM);
            Object.DefineModule(FLOAT);
            Object.DefineModule(HASH);
            Object.DefineModule(NIL);
            Object.DefineModule(RANGE);
            Object.DefineModule(RATIONAL);
            Object.DefineModule(REGEXP);
            Object.DefineModule(STRING);
            Object.DefineModule(SYMBOL);
            Object.DefineModule(TRUE);
            
            Eq = CreateCallSite("==", ParameterKind.Req);
        }
        
        private static CallSite CreateCallSite(string methodName, params ParameterKind[] kinds) =>
            new CallSite(new Symbol(methodName), kinds);
    }
}


