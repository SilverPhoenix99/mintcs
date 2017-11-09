using System;
using System.Linq;
using Mint.Reflection.Parameters;
using Mint.Reflection;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mint.Compilation.Components
{
    internal partial class MethodCompiler
    {
        private class DelegateMetadataBuilder
        {
            private readonly Delegate Lambda;
            private readonly string Name;
            private readonly Parameter[] Parameters;

            public DelegateMetadataBuilder(Delegate lambda, string name, Parameter[] parameters)
            {
                Lambda = lambda ?? throw new ArgumentNullException(nameof(lambda));
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            }

            public DelegateMetadata Build()
            {
                var methodMetadata = new MethodMetadata(
                    Lambda.Method,
                    Name,
                    hasInstance: true,
                    parameters: BuildParameterMetadatas()
                );

                return new DelegateMetadata(Lambda, methodMetadata);
            }

            private IEnumerable<ParameterMetadata> BuildParameterMetadatas()
            {
                var hasClosure = Lambda.Method.GetParameters().Any(p => p.ParameterType == typeof(Closure));
                var offset = hasClosure ? 2 : 1;

                var parameterInfos = Lambda.Method.GetParameters().Skip(offset);

                return Parameters.Zip(parameterInfos, (p, i) => BuildParameterMetadata(p, i, offset));
            }

            private static ParameterMetadata BuildParameterMetadata(Parameter parameter, ParameterInfo info, int offset)
            {
                return new ParameterMetadata(
                    info,
                    parameter.Name.Name,
                    info.Position - offset,
                    parameter.Kind.GetAttributes()
                );
            }
        }
    }
}
