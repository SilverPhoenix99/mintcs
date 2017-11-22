using System;
using System.Linq;
using Mint.MethodBinding;
using Mint.Reflection;
using Mint.Reflection.Parameters.Attributes;

namespace Mint
{
    [RubyModule]
    public static class Kernel
    {
        /*
        ::methods:
          __callee__  autoload?         exec    global_variables  open    rand              sleep    throw
          __dir__     binding           exit    Hash              p       Rational          spawn    trace_var
          __method__  block_given?      exit!   Integer           print   readline          sprintf  trap
          `           caller            fail    iterator?         printf  readlines         srand    untrace_var
          abort       caller_locations  Float   lambda            proc    require           String   warn
          Array       catch             fork    load              putc    require_relative  syscall
          at_exit     Complex           format  local_variables   puts    select            system
          autoload    eval              gets    loop              raise   set_trace_func    test
        #methods:
          __callee__        gets              readlines            class                       object_id
          __dir__           global_variables  require              clone                       private_methods
          __method__        Hash              require_relative     define_singleton_method     protected_methods
          `                 initialize_clone  respond_to_missing?  display                     public_method
          abort             initialize_copy   select               dup                         public_methods
          Array             initialize_dup    set_trace_func       enum_for                    public_send
          at_exit           Integer           sleep                eql?                        remove_instance_variable
          autoload          iterator?         spawn                extend                      respond_to?
          autoload?         lambda            sprintf              freeze                      send
          binding           load              srand                frozen?                     singleton_class
          block_given?      local_variables   String               hash                        singleton_method
          caller            loop              syscall              inspect                     singleton_methods
          caller_locations  open              system               instance_of?                taint
          catch             p                 test                 instance_variable_defined?  tainted?
          Complex           print             throw                instance_variable_get       tap
          eval              printf            trace_var            instance_variable_set       to_enum
          exec              proc              trap                 instance_variables          to_s
          exit              putc              untrace_var          is_a?                       trust
          exit!             puts              warn                 itself                      untaint
          fail              raise             !~                   kind_of?                    untrust
          Float             rand              <=>                  method                      untrusted?
          fork              Rational          ===                  methods
          format            readline          =~                   nil?
        */

        [RubyMethod("__callee__", Visibility = Visibility.Private)]
        [RubyMethod("__dir__", Visibility = Visibility.Private)]
        [RubyMethod("__method__", Visibility = Visibility.Private)]
        [RubyMethod("`", Visibility = Visibility.Private)]
        [RubyMethod("abort", Visibility = Visibility.Private)]
        [RubyMethod("Array", Visibility = Visibility.Private)]
        [RubyMethod("at_exit", Visibility = Visibility.Private)]
        [RubyMethod("autoload", Visibility = Visibility.Private)]
        [RubyMethod("autoload?", Visibility = Visibility.Private)]
        [RubyMethod("binding", Visibility = Visibility.Private)]
        [RubyMethod("block_given?", Visibility = Visibility.Private)]
        [RubyMethod("caller", Visibility = Visibility.Private)]
        [RubyMethod("caller_locations", Visibility = Visibility.Private)]
        [RubyMethod("catch", Visibility = Visibility.Private)]
        [RubyMethod("Complex", Visibility = Visibility.Private)]
        [RubyMethod("eval", Visibility = Visibility.Private)]
        [RubyMethod("exec", Visibility = Visibility.Private)]
        [RubyMethod("exit", Visibility = Visibility.Private)]
        [RubyMethod("exit!", Visibility = Visibility.Private)]
        [RubyMethod("fail", Visibility = Visibility.Private)]
        [RubyMethod("Float", Visibility = Visibility.Private)]
        [RubyMethod("fork", Visibility = Visibility.Private)]
        [RubyMethod("format", Visibility = Visibility.Private)]
        [RubyMethod("gets", Visibility = Visibility.Private)]
        [RubyMethod("global_variables", Visibility = Visibility.Private)]
        [RubyMethod("Hash", Visibility = Visibility.Private)]
        [RubyMethod("initialize_clone", Visibility = Visibility.Private)]
        [RubyMethod("initialize_copy", Visibility = Visibility.Private)]
        [RubyMethod("initialize_dup", Visibility = Visibility.Private)]
        [RubyMethod("Integer", Visibility = Visibility.Private)]
        [RubyMethod("iterator?", Visibility = Visibility.Private)]
        [RubyMethod("lambda", Visibility = Visibility.Private)]
        [RubyMethod("load", Visibility = Visibility.Private)]
        [RubyMethod("local_variables", Visibility = Visibility.Private)]
        [RubyMethod("loop", Visibility = Visibility.Private)]
        [RubyMethod("open", Visibility = Visibility.Private)]
        [RubyMethod("p", Visibility = Visibility.Private)]
        [RubyMethod("print", Visibility = Visibility.Private)]
        [RubyMethod("printf", Visibility = Visibility.Private)]
        [RubyMethod("proc", Visibility = Visibility.Private)]
        [RubyMethod("putc", Visibility = Visibility.Private)]
        [RubyMethod("puts", Visibility = Visibility.Private)]
        [RubyMethod("raise", Visibility = Visibility.Private)]
        [RubyMethod("rand", Visibility = Visibility.Private)]
        [RubyMethod("Rational", Visibility = Visibility.Private)]
        [RubyMethod("readline", Visibility = Visibility.Private)]
        [RubyMethod("readlines", Visibility = Visibility.Private)]
        [RubyMethod("require", Visibility = Visibility.Private)]
        [RubyMethod("require_relative", Visibility = Visibility.Private)]
        [RubyMethod("respond_to_missing?", Visibility = Visibility.Private)]
        [RubyMethod("select", Visibility = Visibility.Private)]
        [RubyMethod("set_trace_func", Visibility = Visibility.Private)]
        [RubyMethod("sleep", Visibility = Visibility.Private)]
        [RubyMethod("spawn", Visibility = Visibility.Private)]
        [RubyMethod("sprintf", Visibility = Visibility.Private)]
        [RubyMethod("srand", Visibility = Visibility.Private)]
        [RubyMethod("String", Visibility = Visibility.Private)]
        [RubyMethod("syscall", Visibility = Visibility.Private)]
        [RubyMethod("system", Visibility = Visibility.Private)]
        [RubyMethod("test", Visibility = Visibility.Private)]
        [RubyMethod("throw", Visibility = Visibility.Private)]
        [RubyMethod("trace_var", Visibility = Visibility.Private)]
        [RubyMethod("trap", Visibility = Visibility.Private)]
        [RubyMethod("untrace_var", Visibility = Visibility.Private)]
        [RubyMethod("warn", Visibility = Visibility.Private)]
        [RubyMethod("!~")]
        [RubyMethod("<=>")]
        [RubyMethod("=~")]
        [RubyMethod("clone")]
        [RubyMethod("define_singleton_method")]
        [RubyMethod("display")]
        [RubyMethod("dup")]
        [RubyMethod("enum_for")]
        [RubyMethod("eql?")]
        [RubyMethod("extend")]
        [RubyMethod("instance_of?")]
        [RubyMethod("instance_variable_defined?")]
        [RubyMethod("method")]
        [RubyMethod("methods")]
        [RubyMethod("private_methods")]
        [RubyMethod("protected_methods")]
        [RubyMethod("public_method")]
        [RubyMethod("public_methods")]
        [RubyMethod("public_send")]
        [RubyMethod("remove_instance_variable")]
        [RubyMethod("respond_to?")]
        [RubyMethod("send")]
        [RubyMethod("singleton_method")]
        [RubyMethod("singleton_methods")]
        [RubyMethod("taint")]
        [RubyMethod("tainted?")]
        [RubyMethod("tap")]
        [RubyMethod("to_enum")]
        [RubyMethod("trust")]
        [RubyMethod("untaint")]
        [RubyMethod("untrust")]
        [RubyMethod("untrusted?")]
        public static iObject NotImplemented(this iObject instance, [Rest] Array args, [Block] object block)
            => throw new NotImplementedException(
                $"{nameof(Kernel)}#{CallFrame.Current.CallSite.MethodName.Name}"
            );

        [RubyMethod("nil?")]
        public static bool IsNil(this iObject instance) => false;

        [RubyMethod("itself")]
        public static iObject Itself(this iObject instance) => instance;

        [RubyMethod("===")]
        public static bool Equals(this iObject left, iObject right) => Object.ToBool(Class.EqOp.Call(left, right));

        [RubyMethod("is_a?")]
        [RubyMethod("kind_of?")]
        public static bool IsA(this iObject instance, iObject arg)
        {
            if(!(arg is Module module))
            {
                throw new TypeError("class or module required");
            }

            var instanceClass = instance as Class ?? instance.EffectiveClass;

            return instanceClass.Ancestors.Any(c => c.Equals(module));
        }

#pragma warning disable CS1720
        internal static ModuleBuilder<iObject> Build() => ModuleBuilder<iObject>.DescribeModule(nameof(Kernel))
            .AutoDefineMethods(typeof(Kernel))

            .AttrReader("class", _ => _.Class )
            .DefMethod("freeze", _ => _.Freeze() )
            .DefMethod("frozen?", Reflector<iObject>.Getter(_ => _.Frozen) )
            .DefMethod("hash", _ => _.GetHashCode() )
            .DefMethod("inspect", _ => _.Inspect() )
            .DefMethod("instance_variable_get", _ => _.InstanceVariableGet(default(Symbol)) )
            .DefMethod("instance_variable_set", _ => _.InstanceVariableSet(default(Symbol), default) )
            .AttrReader("instance_variables", _ => _.InstanceVariables )
            .AttrReader("object_id", _ => _.Id )
            .AttrReader("singleton_class", _ => _.SingletonClass )
            .DefMethod("to_s", _ => _.ToString() )
        ;
#pragma warning restore CS1720
    }
}