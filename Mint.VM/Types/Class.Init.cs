using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mint
{
    public partial class Class : Module
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

        static Class()
        {
            BASIC_OBJECT = ClassBuilder<Object>.Describe(null, "BasicObject")
                //.DefLambda("!", (instance, _) => Box(!ToBool(instance)), new Range(new Fixnum(0), new Fixnum(0)) )
            ;

            // TODO define in Kernel module
            OBJECT = ClassBuilder<Object>.Describe(BASIC_OBJECT)
                .DefMethod("class",   Reflector.Getter( () => ((iObject) null).Class ) )
                .DefMethod("to_s",    () => ((FrozenObject) null).ToString())
                .DefMethod("inspect", () => ((FrozenObject) null).Inspect())
            ;

            MODULE = ClassBuilder<Module>.Describe()
                .DefMethod("to_s",    _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            CLASS = ClassBuilder<Class>.Describe(MODULE);

            // required hack
            CLASS.calculatedClass = CLASS;

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
        }
    }
}


