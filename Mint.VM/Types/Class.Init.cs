using Mint.MethodBinding;
using Mint.MethodBinding.Arguments;
using System;
using System.Diagnostics;

// ReSharper disable EqualExpressionComparison

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

        internal static readonly CallSite EqOp;

#pragma warning disable CS1720

        static Class()
        {
            EqOp = new CallSite(Symbol.EQ, Visibility.Private, ArgumentKind.Simple);

            BASIC_OBJECT = ModuleBuilder<Object>.DescribeClass(null, "BasicObject")
                .Allocator( () => new Object(BASIC_OBJECT) )
                .DefMethod("equal?", () => ReferenceEquals(default(object), default(object)) )
                .AttrReader("__id__", () => default(iObject).Id )
                .DefLambda("!", (Func<iObject, bool>) (_ => !Object.ToBool(_)) )
                .Alias("==", "equal?")
                .DefLambda("!=", (Func<iObject, iObject, bool>) ((l, r) => !Object.ToBool(EqOp.Call(l, r))) )
            ;

            Debug.Assert(BASIC_OBJECT.BaseName != null, $"{nameof(BASIC_OBJECT)}.{nameof(BaseName)} != null");
            BASIC_OBJECT.SetConstant(BASIC_OBJECT.BaseName.Value, BASIC_OBJECT);

            OBJECT = ModuleBuilder<Object>.DescribeClass(BASIC_OBJECT)
                .GenerateAllocator()
            ;
            
            OBJECT.SetConstant(BASIC_OBJECT.BaseName.Value, BASIC_OBJECT);
            OBJECT.SetConstant(OBJECT.BaseName.Value, OBJECT);

            MODULE = ModuleBuilder<Module>.DescribeClass()
                .GenerateAllocator()
                .AttrReader("ancestors", _ => _.Ancestors )
                .DefMethod("const_defined?", _ => _.IsConstantDefined(default(Symbol), default(bool)) )
                .DefMethod("const_get", _ => _.GetConstant(default(Symbol), default(bool)) )
                .DefMethod("const_set", _ => _.SetConstant(default(Symbol), default(iObject)) )
                .AttrReader("constants", _ => _.Constants )
                .DefMethod("inspect", _ => _.Inspect() )
                .AttrReader("name", _ => _.Name )
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("==", () => ReferenceEquals(default(object), default(object)) )
                .DefLambda("===", (Func<iObject, iObject, bool>) ((mod, arg) => Object.IsA(arg, mod)) )
            ;

            CLASS = ModuleBuilder<Class>.DescribeClass(MODULE)
                .GenerateAllocator()
                .DefMethod("allocate", _ => _.Allocate() )
                .AttrReader("superclass", _ => _.Superclass )
            ;

            // required hack
            // otherwise effectiveClass will be null
            BASIC_OBJECT.effectiveClass = CLASS;
            OBJECT.effectiveClass = CLASS;
            MODULE.effectiveClass = CLASS;
            CLASS.effectiveClass = CLASS;

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
                .DefLambda("===", (Func<iObject, iObject, bool>) ((l, r) => Object.ToBool(EqOp.Call(l, r))) )
                .DefMethod("is_a?", () => Object.IsA(default(iObject), default(iObject)) )
                .Alias("kind_of?", "is_a?")
                .DefMethod("instance_variable_get", () => default(iObject).InstanceVariableGet(default(Symbol)) )
                .DefMethod("instance_variable_set", () =>
                    default(iObject).InstanceVariableSet(default(Symbol), default(iObject))
                )
                .AttrReader("instance_variables", () => default(iObject).InstanceVariables )
            ;

            OBJECT.Include(kernel);

            NUMERIC = ModuleBuilder<Object>.DescribeClass("Numeric")
                .Allocator(() => new Object(NUMERIC))
            ;

            FLOAT = ModuleBuilder<Float>.DescribeClass(NUMERIC)
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("inspect", _ => _.Inspect() )
                .DefLambda("abs", (Func<Float, Float>) (_ => Math.Abs(_.Value)) )
                .DefMethod("+", () => default(Float) + default(Float) )
                .DefMethod("-", () => default(Float) - default(Float) )
                .DefMethod("*", () => default(Float) * default(Float) )
                .DefMethod("/", () => default(Float) / default(Float) )
                .DefMethod("==", _ => _.Equals(default(object)) )
                .Alias("===", "==")
            ;

            COMPLEX = ModuleBuilder<Complex>.DescribeClass(NUMERIC)
                //TODO .DefMethod("==", _ => _.Equals(default(object)) )
            ;

            RATIONAL = ModuleBuilder<Rational>.DescribeClass(NUMERIC)
                //TODO .DefMethod("==", _ => _.Equals(default(object)) )
            ;

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
                .DefLambda("size", (Func<Fixnum, Fixnum>) (_ => Fixnum.BYTE_SIZE) )
                .DefMethod("bit_length", _ => _.BitLength() )
                .DefMethod("+", () => default(Fixnum) + default(Fixnum) )
                .DefMethod("-", () => default(Fixnum) - default(Fixnum) )
                .DefMethod("*", () => default(Fixnum) * default(Fixnum) )
                .DefMethod("/", () => default(Fixnum) / default(Fixnum) )
                .DefMethod("==", _ => _.Equals(default(object)) )
                .Alias("===", "==")
            ;

            BIGNUM = ModuleBuilder<Bignum>.DescribeClass(INTEGER)
                .DefMethod("==", _ => _.Equals(default(object)) )
            ;

            BINDING = ModuleBuilder<Binding>.DescribeClass()
                // TODO: eval
                .DefMethod("clone", _ => _.Duplicate() )
                .Alias("dup", "clone")
                .DefMethod("local_variable_defined?", _ => _.IsLocalDefined(default(Symbol)) )
                .DefMethod("local_variable_get", _ => _.GetLocalValue(default(Symbol)) )
                .DefMethod("local_variable_set", _ => _.SetLocalValue(default(Symbol), default(iObject)) )
                .AttrReader("local_variables", _ => _.LocalVariables )
                .AttrReader("receiver", _ => _.Receiver )
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
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("inspect", _ => _.Inspect() )
            ;

            TRUE = ModuleBuilder<TrueClass>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("inspect", _ => _.Inspect() )
            ;

            ARRAY = ModuleBuilder<Array>.DescribeClass()
                .GenerateAllocator()
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("inspect", _ => _.Inspect() )
                .AttrAccessor("[]", _ => _[default(int)] )
                .DefMethod("clear", _ => _.Clear() )
                .DefMethod("join", _ => _.Join(default(String)) )
                .DefMethod("replace", _ => _.Replace(default(Array)) )
                .DefMethod("compact", _ => _.Compact() )
                .DefMethod("compact!", _ => _.CompactSelf() )
                .DefMethod("reverse", _ => _.Reverse() )
                .DefMethod("reverse!", _ => _.ReverseSelf() )
                .DefMethod("first", _ => _.First() )
                .DefMethod("last", _ => _.Last() )
                .DefMethod("uniq", _ => _.Uniq() )
                .DefMethod("uniq!", _ => _.UniqSelf() )
                .DefMethod("<<", _ => _.Add(default(iObject)) )
                .DefMethod("&", _ => _.AndAlso(default(Array)) )
                .DefMethod("==", _ => _.Equals(default(iObject)) )
            ;

            HASH = ModuleBuilder<Hash>.DescribeClass()
                .GenerateAllocator()
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("inspect", _ => _.Inspect() )
                .AttrReader("count", _ => _.Count )
                .AttrAccessor("[]", _ => _[default(iObject)] )
                .DefMethod("merge", _ => _.Merge(default(Hash)) )
                .DefMethod("merge!", _ => _.MergeSelf(default(Hash)) )
            ;

            PROC = ModuleBuilder<Proc>.DescribeClass()
            ;

            RANGE = ModuleBuilder<Range>.DescribeClass()
                .GenerateAllocator()
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("inspect", _ => _.Inspect() )
            ;

            REGEXP = ModuleBuilder<Regexp>.DescribeClass()
                .GenerateAllocator()
            ;

            STRING = ModuleBuilder<String>.DescribeClass()
                .GenerateAllocator()
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("inspect", _ => _.Inspect() )
                .DefMethod("ljust", _ => _.LeftJustify(default(int), default(string)) )
            ;

            SYMBOL = ModuleBuilder<Symbol>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("inspect", _ => _.Inspect() )
            ;
        }

#pragma warning restore CS1720

    }
}