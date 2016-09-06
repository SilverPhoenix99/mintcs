using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
         *     return Object.Box(<Type>.<Method>($instance, (<cast>) $arguments[0], ...));
         * }
         */
        private class StaticCallEmitter : InstanceCallEmitter
        {
            protected override int Offset => 1;

            public StaticCallEmitter(MethodInfo method, CallFrameBinder bundledFrame, ParameterExpression argumentsArray)
                : base(method, bundledFrame, argumentsArray)
            { }

            protected override Type TryGetInstanceType() => ParameterInfos[0].ParameterType;

            protected override Expression GetInstance() => null;

            protected override IEnumerable<Expression> GetArguments()
            {
                var convertedArgs = ParameterInfos.Skip(1).Select(ConvertArgument);
                return new[] { GetConvertedInstance() }.Concat(convertedArgs);
            }
        }
    }
}