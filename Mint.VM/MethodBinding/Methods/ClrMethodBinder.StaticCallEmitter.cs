using Mint.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Mint.MethodBinding.Methods
{
    public sealed partial class ClrMethodBinder
    {
        /*
         * Generated Stub:
         *
         * global iObject $instance;
         * global ArgumentBundle $bundle;
         * global iObject[] $arguments;
         *
         * case {
         *     $arguments = $bundle.Bind(@methodInfo);
         *     $arguments != null && $arguments[0] is <Type> && ...
         * }:
         * {
         *     // TODO: add arguments as local variables
         *     return Object.Box(<Type>.<Method>((<cast>) $instance, (<cast>) $arguments[0], ...));
         * }
         */
        private class StaticCallEmitter : InstanceCallEmitter
        {
            public StaticCallEmitter(MethodMetadata method, CallFrameBinder frame)
                : base(method, frame)
            { }
            
            protected override Expression GetInstance() => null;

            protected override Expression GetConvertedInstance()
            {
                var position = Method.HasClosure ? 1 : 0;
                var type = Method.Method.GetParameters()[position].ParameterType;
                return Frame.Instance.Cast(type);
            }

            protected override IEnumerable<Expression> GetArguments()
            {
                var convertedArgs = base.GetArguments();
                return new[] { GetConvertedInstance() }.Concat(convertedArgs);
            }
        }
    }
}