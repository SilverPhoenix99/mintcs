using System;
using Mint.MethodBinding;
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
        [RubyMethod("===")]
        [RubyMethod("=~")]
        [RubyMethod("class")]
        [RubyMethod("clone")]
        [RubyMethod("define_singleton_method")]
        [RubyMethod("display")]
        [RubyMethod("dup")]
        [RubyMethod("enum_for")]
        [RubyMethod("eql?")]
        [RubyMethod("extend")]
        [RubyMethod("freeze")]
        [RubyMethod("frozen?")]
        [RubyMethod("hash")]
        [RubyMethod("inspect")]
        [RubyMethod("instance_of?")]
        [RubyMethod("instance_variable_defined?")]
        [RubyMethod("instance_variable_get")]
        [RubyMethod("instance_variable_set")]
        [RubyMethod("instance_variables")]
        [RubyMethod("is_a?")]
        [RubyMethod("itself")]
        [RubyMethod("kind_of?")]
        [RubyMethod("method")]
        [RubyMethod("methods")]
        [RubyMethod("nil?")]
        [RubyMethod("object_id")]
        [RubyMethod("private_methods")]
        [RubyMethod("protected_methods")]
        [RubyMethod("public_method")]
        [RubyMethod("public_methods")]
        [RubyMethod("public_send")]
        [RubyMethod("remove_instance_variable")]
        [RubyMethod("respond_to?")]
        [RubyMethod("send")]
        [RubyMethod("singleton_class")]
        [RubyMethod("singleton_method")]
        [RubyMethod("singleton_methods")]
        [RubyMethod("taint")]
        [RubyMethod("tainted?")]
        [RubyMethod("tap")]
        [RubyMethod("to_enum")]
        [RubyMethod("to_s")]
        [RubyMethod("trust")]
        [RubyMethod("untaint")]
        [RubyMethod("untrust")]
        [RubyMethod("untrusted?")]
        public static iObject NotImplemented(this iObject instance, [Rest] Array args)
            => throw new NotImplementedException();

        internal static ModuleBuilder<iObject> Build() => ModuleBuilder<iObject>.DescribeModule(nameof(Kernel))
            .AutoDefineMethods(typeof(Kernel))
        ;
    }
}