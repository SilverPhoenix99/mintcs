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
            
            BASIC_OBJECT = BasicObject.Build();

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
                .DefMethod("const_defined?", _ => _.IsConstantDefined(default(Symbol), default) )
                .DefMethod("const_get", _ => _.GetConstant(default(Symbol), default) )
                .DefMethod("const_set", _ => _.SetConstant(default, default) )
                .AttrReader("constants", _ => _.Constants )
                .DefMethod("inspect", _ => _.Inspect() )
                .AttrReader("name", _ => _.Name )
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("==", () => ReferenceEquals(default, default) )
                .DefLambda("===", (Func<iObject, iObject, bool>) ((mod, arg) => arg.IsA(mod)) )
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

            OBJECT.Include(Kernel.Build());

            NUMERIC = ModuleBuilder<Object>.DescribeClass("Numeric")
                .Allocator(() => new Object(NUMERIC))
            ;

            FLOAT = Float.Build();

            COMPLEX = ModuleBuilder<Complex>.DescribeClass(NUMERIC)
                //TODO .DefMethod("==", _ => _.Equals(default(object)) )
            ;

            RATIONAL = ModuleBuilder<Rational>.DescribeClass(NUMERIC)
                //TODO .DefMethod("==", _ => _.Equals(default(object)) )
            ;

            INTEGER = new Class(NUMERIC, new Symbol("Integer"));

            FIXNUM = Fixnum.Build();
            
            BIGNUM = ModuleBuilder<Bignum>.DescribeClass(INTEGER)
                .DefMethod("==", _ => _.Equals(default(object)) )
            ;

            BINDING = ModuleBuilder<Binding>.DescribeClass()
                // TODO: eval
                .DefMethod("clone", _ => _.Duplicate() )
                .Alias("dup", "clone")
                .DefMethod("local_variable_defined?", _ => _.IsLocalDefined(default) )
                .DefMethod("local_variable_get", _ => _.GetLocalValue(default) )
                .DefMethod("local_variable_set", _ => _.SetLocalValue(default, default) )
                .AttrReader("local_variables", _ => _.LocalVariables )
                .AttrReader("receiver", _ => _.Receiver )
            ;

            NIL = NilClass.Build();
            FALSE = FalseClass.Build();
            TRUE = TrueClass.Build();
            ARRAY = Array.Build();

            HASH = ModuleBuilder<Hash>.DescribeClass()
                .GenerateAllocator()
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("inspect", _ => _.Inspect() )
                .AttrReader("count", _ => _.Count )
                .AttrAccessor("[]", _ => _[default] )
                .DefMethod("merge", _ => _.Merge(default) )
                .DefMethod("merge!", _ => _.MergeSelf(default) )
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
                .DefLambda("to_s", (Func<String, String>) (_ => _) ) // Optimization: return itself
                .DefMethod("inspect", _ => _.Inspect() )
                .DefMethod("ljust", _ => _.LeftJustify(default, default) )
            ;

            SYMBOL = ModuleBuilder<Symbol>.DescribeClass()
                .DefMethod("to_s", _ => _.ToString() )
                .DefMethod("inspect", _ => _.Inspect() )
            ;
        }
#pragma warning restore CS1720
    }
}