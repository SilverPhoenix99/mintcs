using System;
using Mint.MethodBinding;
using Mint.Reflection.Parameters.Attributes;

namespace Mint
{
    [RubyClass(Superclass = null)]
    public static class BasicObject
    {
        /*
        constants:
            BasicObject
        #methods:
            !   ==      __send__  initialize     instance_exec   singleton_method_added    singleton_method_undefined
            !=  __id__  equal?    instance_eval  method_missing  singleton_method_removed
        */

        private static iObject Allocate() => new Object(Class.BASIC_OBJECT);

        [RubyMethod("__send__")]
        [RubyMethod("initialize", Visibility = Visibility.Private)]
        [RubyMethod("instance_eval")]
        [RubyMethod("instance_exec")]
        [RubyMethod("method_missing", Visibility = Visibility.Private)]
        [RubyMethod("singleton_method_added", Visibility = Visibility.Private)]
        [RubyMethod("singleton_method_removed", Visibility = Visibility.Private)]
        [RubyMethod("singleton_method_undefined", Visibility = Visibility.Private)]
        public static iObject NotImplemented(this iObject instance, [Rest] Array args, [Block] object block)
            => throw new NotImplementedException(
                $"{nameof(BasicObject)}#{CallFrame.Current.CallSite.MethodName.Name}"
            );

        [RubyMethod("!")]
        public static bool Not(this iObject instance) => !Object.ToBool(instance);

        [RubyMethod("!=")]
        public static bool NotEq(this iObject instance, iObject other)
            => !Object.ToBool( Class.EqOp.Call(instance, other) );

#pragma warning disable CS1720
        internal static ModuleBuilder<Object> Build() =>
            ModuleBuilder<Object>.DescribeClass(null, "BasicObject")
                .Allocator(Allocate)
                .AutoDefineMethods(typeof(BasicObject))

                .AttrReader("__id__",
                    () => default(iObject).Id
                )
                
                .DefMethod("==",
                    // ReSharper disable once EqualExpressionComparison
                    () => ReferenceEquals(default, default)
                )
                
                .Alias("equal?", "==")
        ;
#pragma warning restore CS1720
    }
}