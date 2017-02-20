using Mint.MethodBinding;
using Mint.MethodBinding.Arguments;
using System;
using System.Diagnostics;

namespace Mint
{
    public partial class Class
    {
        public static readonly Class BASIC_OBJECT;
        public static readonly Class OBJECT;
        public static readonly Class MODULE;
        public static readonly Class CLASS;
        public static readonly Class ARRAY;
        public static readonly Class BIGNUM;
        public static readonly Class BINDING;
        public static readonly Class COMPLEX;
        public static readonly Class FALSE;
        public static readonly Class FIXNUM;
        public static readonly Class FLOAT;
        public static readonly Class HASH;
        public static readonly Class INTEGER;
        public static readonly Class NIL;
        public static readonly Class NUMERIC;
        public static readonly Class PROC;
        public static readonly Class RANGE;
        public static readonly Class RATIONAL;
        public static readonly Class REGEXP;
        public static readonly Class STRING;
        public static readonly Class SYMBOL;
        public static readonly Class TRUE;

        private static readonly CallSite EqOp;

        static Class()
        {
            EqOp = new CallSite(Symbol.EQ, Visibility.Private, ArgumentKind.Simple);

            #pragma warning disable 1720
            BASIC_OBJECT = ModuleBuilder<Object>.DescribeClass(null, "BasicObject")
                .Allocator( () => new Object(BASIC_OBJECT) )
                .DefMethod( "equal?", () => default(iObject).Equal(default(object)) )
                .AttrReader("__id__", () => default(iObject).Id )
                .DefLambda("!", (Func<iObject, bool>) (_ => !Object.ToBool(_)) )
                .DefMethod("==", () => default(iObject).Equals(default(object)) )
                .DefLambda("!=", (Func<iObject, iObject, bool>) ( (l, r) => !Object.ToBool(EqOp.Call(l, r)) ) )
            ;
            #pragma warning restore 1720

            Debug.Assert(BASIC_OBJECT.Name != null, "BASIC_OBJECT.Name != null");
            BASIC_OBJECT.SetConstant(BASIC_OBJECT.Name.Value, BASIC_OBJECT);

            #pragma warning disable 1720
            OBJECT = ModuleBuilder<Object>.DescribeClass(BASIC_OBJECT)
                .Allocator( () => new Object() )
                .DefMethod("instance_variable_get", () => default(iObject).InstanceVariableGet(default(Symbol)))
                .DefMethod("instance_variable_set", () =>
                    default(iObject).InstanceVariableSet(default(Symbol), default(iObject))
                )
                .AttrReader("instance_variables", () => default(iObject).InstanceVariables)
            ;
            #pragma warning restore 1720

            OBJECT.SetConstant(BASIC_OBJECT.Name.Value, BASIC_OBJECT);
            OBJECT.SetConstant(OBJECT.Name.Value, OBJECT);

            MODULE = ModuleBuilder<Module>.DescribeClass()
                .Allocator( () => new Module() )
                .AttrReader("ancestors", _ => _.Ancestors)
                .DefMethod("const_defined?", _ => _.IsConstantDefined(default(Symbol), default(bool)))
                .DefMethod("const_get", _ => _.GetConstant(default(Symbol), default(bool)))
                .DefMethod("const_set", _ => _.SetConstant(default(Symbol), default(iObject)))
                .AttrReader("constants", _ => _.Constants)
                .DefMethod("inspect", _ => _.Inspect())
                .AttrReader("name", _ => _.FullName)
                .DefMethod("to_s", _ => _.ToString())
            ;

            CLASS = ModuleBuilder<Class>.DescribeClass(MODULE)
                .Allocator( () => new Class() )
                .DefMethod("allocate", _ => _.Allocate())
                .AttrReader("superclass", _ => _.Superclass)
            ;

            // required hack
            // otherwise effectiveClass will be null
            BASIC_OBJECT.effectiveClass = CLASS;
            OBJECT.effectiveClass = CLASS;
            MODULE.effectiveClass = CLASS;
            CLASS.effectiveClass = CLASS;

            #pragma warning disable 1720
            Module kernel = ModuleBuilder<iObject>.DescribeModule("Kernel")
                .AttrReader("class", _ => _.Class )
                .AttrReader("singleton_class", _ => _.SingletonClass )
                .DefMethod("to_s", () => default(FrozenObject).ToString() )
                .DefMethod("inspect", () => default(FrozenObject).Inspect() )
                .DefLambda("nil?", (Func<iObject, bool>) (_ => false) )
                .DefLambda("frozen?", (Func<iObject, bool>) (_ => _.Frozen) )
                .DefMethod("freeze", _ => _.Freeze() )
                .DefMethod("hash", _ => _.GetHashCode() )
                .DefLambda("itself", (Func<iObject, iObject>) (_ => _) )
                .AttrReader("object_id", _ => _.Id )
                //.DefMethod("to_bool", () => Object.ToBool(default(iObject)) ) // for testing static methods
            ;
            #pragma warning restore 1720

            OBJECT.Include(kernel);

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
                .DefMethod("+", () => default(Fixnum) + default(Fixnum))
            ;

            BIGNUM = ModuleBuilder<Bignum>.DescribeClass(INTEGER)
            ;

            BINDING = ModuleBuilder<Binding>.DescribeClass()
                // TODO: eval
                .DefMethod("clone", _ => _.Duplicate())
                .Alias("dup", "clone")
                .DefMethod("local_variable_defined?", _ => _.IsLocalDefined(default(Symbol)))
                .DefMethod("local_variable_get", _ => _.GetLocalValue(default(Symbol)))
                .DefMethod("local_variable_set", _ => _.SetLocalValue(default(Symbol), default(iObject)))
                .AttrReader("local_variables", _ => _.LocalVariables)
                .AttrReader("receiver", _ => _.Receiver)
            ;

            NIL = ModuleBuilder<NilClass>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("inspect", _ => _.Inspect() )
                .DefLambda("nil?", (Func<iObject, bool>) (_ => true) )
                .DefLambda("to_a", (Func<iObject, Array>) (_ => new Array()) )
                .DefLambda("to_f", (Func<iObject, Float>) (_ => new Float(0.0)) )
                .DefLambda("to_i", (Func<iObject, Fixnum>) (_ => new Fixnum()) )
                .DefLambda("to_h", (Func<iObject, Hash>) (_ => new Hash()) )
            ;

            FALSE = ModuleBuilder<FalseClass>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            TRUE = ModuleBuilder<TrueClass>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            ARRAY = ModuleBuilder<Array>.DescribeClass()
                .Allocator( () => new Mint.Array() )
                .DefMethod("to_s", _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
                .AttrAccessor("[]", _ => _[default(int)])
                .DefMethod("clear", _ => _.Clear())
                .DefMethod("join", _ => _.Join(default(String)))
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
                .DefMethod("&", _ => _.AndAlso(default(Array)))
            ;

            HASH = ModuleBuilder<Hash>.DescribeClass()
                .Allocator( () => new Hash() )
                .DefMethod("to_s", _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
                .AttrReader("count", _ => _.Count)
                .AttrAccessor("[]", _ => _[default(iObject)])
                .DefMethod("merge", _ => _.Merge(default(Hash)))
                .DefMethod("merge!", _ => _.MergeSelf(default(Hash)))
            ;

            PROC = ModuleBuilder<Proc>.DescribeClass()
            ;

            RANGE = ModuleBuilder<Range>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;

            REGEXP = ModuleBuilder<Regexp>.DescribeClass();

            STRING = ModuleBuilder<String>.DescribeClass()
                .Allocator( () => new String() )
                .DefMethod("to_s", _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
                .DefMethod("ljust", _ => _.LeftJustify(default(int), default(string)))
            ;

            SYMBOL = ModuleBuilder<Symbol>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString())
                .DefMethod("inspect", _ => _.Inspect())
            ;
        }
    }
}