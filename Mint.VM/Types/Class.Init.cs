using Mint.Binding;
using Mint.Binding.Arguments;
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
        public static readonly Class BIGNUM;
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

        private static readonly CallSite EqOp;

        static Class()
        {
            EqOp = CallSite.Create(Symbol.EQ, Visibility.Private, ArgumentKind.Simple);

            // ==       |   iObject#Equals(iObject) (by default equal?)
            // equal?   |   object::ReferenceEquals(object, object)
            // eql?     |   (iObject i, iObject a) => i.hash == a.hash

            BASIC_OBJECT = ModuleBuilder<Object>.DescribeClass(null, "BasicObject")
                .DefMethod( "equal?", () => ((iObject) null).Equal(default(object)) )
                .AttrReader("__id__", () => default(iObject).Id )
                .DefLambda("!", (Func<iObject, bool>) (_ => !Object.ToBool(_)) )
                .DefMethod("==", () => default(iObject).Equals(default(object)) )
                .DefLambda("!=", (Func<iObject, iObject, bool>) ( (l, r) => !Object.ToBool(EqOp.Call(l, r)) ) )
                .DefMethod("equal?", () => default(iObject).Equal(default(object)) )
            ;

            BASIC_OBJECT.Constants[BASIC_OBJECT.Name.Value] = BASIC_OBJECT;

            OBJECT = ModuleBuilder<Object>.DescribeClass(BASIC_OBJECT);

            MODULE = ModuleBuilder<Module>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            CLASS = ModuleBuilder<Class>.DescribeClass(MODULE);

            // required hack
            // otherwise CLASS.effectiveClass will be null
            CLASS.effectiveClass = CLASS;

            KERNEL = ModuleBuilder<iObject>.DescribeModule("Kernel")
                .AttrReader("class", _ => _.Class )
                .DefMethod("to_s", () => default(FrozenObject).ToString() )
                .DefMethod("inspect", () => default(FrozenObject).Inspect() )
                .DefLambda("nil?", (Func<iObject, iObject>) (_ => new FalseClass()) )
                .DefLambda("frozen?", (Func<iObject, bool>) (_ => _.Frozen) )
                .DefLambda("freeze", (Func<iObject, iObject>) (_ => { _.Freeze(); return new NilClass(); }) )
                .DefLambda("hash", (Func<iObject, long>) (_ => _.GetHashCode()) )
                .DefLambda("itself", (Func<iObject, iObject>) (_ => _) )
                .AttrReader("object_id", () => default(iObject).Id )
            ;

            OBJECT.Include(KERNEL);
            Object.DefineModule(KERNEL);

            NUMERIC = new Class(new Symbol("Numeric"));

            FLOAT = ModuleBuilder<Float>.DescribeClass(NUMERIC)
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("inspect", _ => _.Inspect() )
                .DefLambda("abs", (Func<Float, Float>) (_ => Math.Abs(_.Value)) )
            ;

            COMPLEX = ModuleBuilder<Complex>.DescribeClass(NUMERIC);

            RATIONAL = ModuleBuilder<Rational>.DescribeClass(NUMERIC);

            INTEGER = new Class(NUMERIC, new Symbol("Integer"));

            FIXNUM = ModuleBuilder<Fixnum>.DescribeClass(INTEGER)
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("inspect", _ => _.Inspect() )
                .DefLambda("abs", (Func<Fixnum, Fixnum>) (_ => Math.Abs(_.Value)) )
                .DefLambda("even?", (Func<Fixnum, bool>) (_ => (_.Value & 1L) == 0L) )
                .DefLambda("odd?", (Func<Fixnum, bool>) (_ => (_.Value & 1L) == 1L) )
                .Alias("magnitude", "abs")
                .DefLambda("to_f", (Func<Fixnum, Float>) (_ => (Float) _) )
                .DefLambda("zero?", (Func<Fixnum, bool>) (_ => _.Value == 0L) )
                .DefLambda("size", (Func<Fixnum, Fixnum>) (_ => Fixnum.SIZE) )
                .DefMethod("bit_length", _ => _.BitLength() )
            ;

            BIGNUM = ModuleBuilder<Bignum>.DescribeClass(INTEGER)
            ;

            NIL = ModuleBuilder<NilClass>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("inspect", _ => _.Inspect() )
                .DefLambda("nil?", (Func<iObject, iObject>) (_ => new TrueClass()) )
                .DefLambda("to_a", (Func<iObject, Array>) (_ => new Array()) )
                .DefLambda("to_f", (Func<iObject, Float>) (_ => new Float(0.0)) )
                .DefLambda("to_i", (Func<iObject, Fixnum>) (_ => new Fixnum()) )
                .DefLambda("to_h", (Func<iObject, Hash>) (_ => new Hash()) )
            ;

            FALSE = ModuleBuilder<FalseClass>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("inspect", _ => _.Inspect() )
            ;

            TRUE = ModuleBuilder<TrueClass>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            ARRAY = ModuleBuilder<Array>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
                .AttrAccessor("[]", _ => _[default(int)])
                .DefMethod("clear", _ => _.Clear())
                .DefMethod("join", _ => _.Join(default(string)))
                .DefMethod("replace", _ => _.Replace(default(Array)))
                .DefMethod("compact", _ => _.Compact())
                .DefMethod("compact!", _ => _.CompactSelf())
                .DefMethod("reverse", _ => _.Reverse())
                .DefMethod("reverse!", _ => _.ReverseSelf())
                .DefMethod("first", _ => _.First())
                .DefMethod("last", _ => _.Last())
                .DefMethod("uniq", _ => _.Uniq())
                .DefMethod("uniq!", _ => _.UniqSelf())
                .DefMethod("<<", _ => _.Add(default(iObject)))
            // .DefMethod("+", () => default(Array) + default(Array))
            ;

            HASH = ModuleBuilder<Hash>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
                .AttrReader("count", _ => _.Count)
                .AttrAccessor("[]", _ => _[default(iObject)])
                .DefMethod("merge", _ => _.Merge(default(Hash)))
                .DefMethod("merge!", _ => _.MergeSelf(default(Hash)))
            ;

            RANGE = ModuleBuilder<Range>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            REGEXP = ModuleBuilder<Regexp>.DescribeClass();

            STRING = ModuleBuilder<String>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
                .DefMethod("ljust", _ => _.LeftJustify(default(int), default(string)))
            ;

            SYMBOL = ModuleBuilder<Symbol>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString())
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
        }
    }
}