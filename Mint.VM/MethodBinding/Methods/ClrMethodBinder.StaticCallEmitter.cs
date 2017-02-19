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
         *     return Object.Box(<Type>.<Method>((<cast>) $instance, (<cast>) $arguments[0], ...));
         * }
         */
        private class StaticCallEmitter : InstanceCallEmitter
        {
            public StaticCallEmitter(MethodMetadata method,
                                     CallFrameBinder bundledFrame,
                                     ParameterExpression argumentsArray)
                : base(method, bundledFrame, argumentsArray)
            { }

            protected override Expression GetInstance() => null;

            protected override IEnumerable<Expression> GetArguments()
            {
                var convertedArgs = base.GetArguments();
                return new[] { GetConvertedInstance() }.Concat(convertedArgs);
            }
        }
    }
}